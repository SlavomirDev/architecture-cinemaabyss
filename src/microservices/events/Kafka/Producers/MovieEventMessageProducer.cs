using CinemaAbyss.Events.Kafka.Factory;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace CinemaAbyss.Events.Kafka.Producers;

public class MovieEventMessageProducer(IOptions<TopicConfiguration> topics, IOptions<KafkaConfiguration> configuration, IKafkaProducerFactory kafkaProducerFactory)
    : KafkaProducer<long, string>(topics.Value.MovieEventsTopicName, configuration, kafkaProducerFactory)
{
    public async Task PublishAsync(MovieEvent message, CancellationToken ct)
    {
        var value = JsonConvert.SerializeObject(message);
        await ProduceAsync(message.MovieId, value, ct);
    }
}