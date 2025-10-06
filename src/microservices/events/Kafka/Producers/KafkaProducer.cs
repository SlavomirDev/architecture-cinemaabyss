using CinemaAbyss.Events.Kafka.Factory;

using Confluent.Kafka;

using Microsoft.Extensions.Options;

namespace CinemaAbyss.Events.Kafka.Producers;

public abstract class KafkaProducer<TKey, TValue>
{
    public string Topic { get; }

    protected readonly IProducer<TKey, TValue> Producer;

    public KafkaProducer(string topic, IOptions<KafkaConfiguration> configuration, IKafkaProducerFactory kafkaProducerFactory)
    {
        Topic = topic;
        Producer = kafkaProducerFactory
            .Build<TKey, TValue>(config =>
            {
                config.AllowAutoCreateTopics = true;
                config.BootstrapServers = configuration.Value.Servers;
            });
    }

    protected virtual async Task ProduceAsync(TKey key, TValue value, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var message = new Message<TKey, TValue>
        {
            Key = key,
            Value = value
        };

        await Producer.ProduceAsync(Topic, message, ct);
    }
}
