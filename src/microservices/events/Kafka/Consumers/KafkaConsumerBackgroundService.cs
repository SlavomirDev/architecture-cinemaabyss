using CinemaAbyss.Events.Kafka.Factory;

using Confluent.Kafka;

using Microsoft.Extensions.Options;

namespace CinemaAbyss.Events.Kafka.Consumers;

public abstract class KafkaConsumerBackgroundService<TKey, TValue> : IHostedService, IDisposable
{
    public string Topic { get; }

    private const int ConsumeTimeout = 100;
    protected readonly IConsumer<TKey, TValue> Consumer;
    private readonly ILogger<KafkaConsumerBackgroundService<TKey, TValue>> _logger;
    private CancellationTokenSource _cts;

    public KafkaConsumerBackgroundService(string topic, IOptions<KafkaConfiguration> configuration, IKafkaConsumerFactory kafkaConsumerFactory, ILogger<KafkaConsumerBackgroundService<TKey, TValue>> logger)
    {
        Topic = topic;
        _logger = logger;
        _cts = new CancellationTokenSource();
        Consumer = kafkaConsumerFactory.Build<TKey, TValue>(config =>
        {
            config.EnableAutoCommit = false;
            config.GroupId = $"{Topic}_group";
            config.AllowAutoCreateTopics = true;
            config.AutoOffsetReset = AutoOffsetReset.Earliest;
            config.BootstrapServers = configuration.Value.Servers;
        });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var token = (_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource().Token)).Token;
        _ = Task.Run(() => ExecuteAsync(token), token);
        return Task.CompletedTask;
    }

    protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        
        Consumer.Subscribe(Topic);
        _logger.LogInformation("Start consumer topic {Topic}", Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ConsumeAsync(stoppingToken);
        }

        Consumer.Unsubscribe();
        _logger.LogInformation("Stop consumer topic {Topic}", Topic);
    }

    protected virtual async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        try
        {
            var consumeResult = Consumer.Consume(TimeSpan.FromMilliseconds(ConsumeTimeout));
            if (consumeResult is not null)
            {
                await HandleAsync(consumeResult, cancellationToken);
                Consumer.Commit();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message.");
        }
    }

    protected abstract Task HandleAsync(ConsumeResult<TKey, TValue> consumeResult, CancellationToken ct);

    public virtual void Dispose()
    {
        Consumer?.Close();
        Consumer?.Dispose();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
