using Confluent.Kafka;

namespace CinemaAbyss.Events.Kafka.Factory;

public class KafkaFactory(ILogger<KafkaFactory> logger) : IKafkaConsumerFactory, IKafkaProducerFactory
{
    private readonly ILogger<KafkaFactory> _logger = logger;

    IConsumer<TKey, TValue> IKafkaConsumerFactory.Build<TKey, TValue>(Action<ConsumerConfig> configAction)
    {
        var config = new ConsumerConfig();
        configAction(config);
        return new ConsumerBuilder<TKey, TValue>(config)
            .SetErrorHandler((_, error) => _logger.LogError(error.Reason))
            .SetLogHandler((_, message) => _logger.LogInformation(message.Message))
            .Build();
    }

    IProducer<TKey, TValue> IKafkaProducerFactory.Build<TKey, TValue>(Action<ProducerConfig> configAction)
    {
        var config = new ProducerConfig();
        configAction(config);
        return new ProducerBuilder<TKey, TValue>(config)
            .SetErrorHandler((_, error) => _logger.LogError(error.Reason))
            .SetLogHandler((_, message) => _logger.LogInformation(message.Message))
            .Build();
    }
}