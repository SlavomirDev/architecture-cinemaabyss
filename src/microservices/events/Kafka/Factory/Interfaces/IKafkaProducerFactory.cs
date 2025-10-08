using Confluent.Kafka;

namespace CinemaAbyss.Events.Kafka.Factory;

public interface IKafkaProducerFactory
{
    IProducer<TKey, TValue> Build<TKey, TValue>(Action<ProducerConfig> configAction);
}