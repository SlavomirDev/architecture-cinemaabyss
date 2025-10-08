using CinemaAbyss.Events.Kafka.Factory;

using Confluent.Kafka;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace CinemaAbyss.Events.Kafka.Consumers;

public class PaymentEventsConsumer : KafkaConsumerBackgroundService<long, string>
{
    private readonly ILogger<PaymentEventsConsumer> _logger;

    public PaymentEventsConsumer(IOptions<TopicConfiguration> topics, IOptions<KafkaConfiguration> configuration, IKafkaConsumerFactory kafkaConsumerFactory, ILogger<PaymentEventsConsumer> logger)
        : base(topics.Value.PaymentEventsTopicName, configuration, kafkaConsumerFactory, logger)
    {
        _logger = logger;
    }

    protected override Task HandleAsync( ConsumeResult<long, string> consumeResult, CancellationToken ct)
    {
        try
        {
            var message = JsonConvert.DeserializeObject<PaymentEvent>(consumeResult.Message.Value);
            if (message is null)
            {
                _logger.LogWarning("Deserializing error: {@Message}", consumeResult.Message.Value);
                return Task.CompletedTask;
            }

            _logger.LogInformation("Processing event with offset {@Offset}: {@Event}", consumeResult.Offset.Value, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handling error: {@ConsumeResult}", consumeResult);
        }

        return Task.CompletedTask;
    }
}