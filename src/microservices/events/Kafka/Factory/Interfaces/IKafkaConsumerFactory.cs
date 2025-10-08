using Confluent.Kafka;

namespace CinemaAbyss.Events.Kafka.Factory;

public interface IKafkaConsumerFactory
{
    IConsumer<TKey, TValue> Build<TKey, TValue>(Action<ConsumerConfig> configAction);
}