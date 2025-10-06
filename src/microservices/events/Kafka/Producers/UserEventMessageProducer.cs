using CinemaAbyss.Events.Kafka.Factory;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace CinemaAbyss.Events.Kafka.Producers;

public class UserEventMessageProducer(IOptions<TopicConfiguration> topics, IOptions<KafkaConfiguration> configuration, IKafkaProducerFactory kafkaProducerFactory)
    : KafkaProducer<long, string>(topics.Value.UserEventsTopicName, configuration, kafkaProducerFactory)
{
    public Task PublishAsync(UserEvent message, CancellationToken ct) => ProduceAsync(message.UserId, JsonConvert.SerializeObject(message), ct);
}