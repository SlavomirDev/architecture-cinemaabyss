namespace CinemaAbyss.Events.Kafka;

public class TopicConfiguration
{
    public string MovieEventsTopicName { get; set; }
    public string UserEventsTopicName { get; set; }
    public string PaymentEventsTopicName { get; set; }
}