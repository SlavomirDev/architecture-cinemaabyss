using CinemaAbyss.Events.Kafka.Factory;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace CinemaAbyss.Events.Kafka.Producers;

public class PaymentEventMessageProducer(IOptions<TopicConfiguration> topics, IOptions<KafkaConfiguration> configuration, IKafkaProducerFactory kafkaProducerFactory)
    : KafkaProducer<long, string>(topics.Value.PaymentEventsTopicName, configuration, kafkaProducerFactory)
{
    public async Task PublishAsync(PaymentEvent message, CancellationToken ct)
    {
        var value = JsonConvert.SerializeObject(message);
        await ProduceAsync(message.PaymentId, value, ct);
    }
}