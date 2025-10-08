namespace CinemaAbyss.Events
{
    public class MovieEvent
    {
        public long MovieId { get; set; }
        public string Title { get; set; }
        public string Action { get; set; }
        public long UserId { get; set; }
    }

    public class PaymentEvent
    {
        public long PaymentId { get; set; }
        public long UserId { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string MethodType { get; set; }
    }

    public class UserEvent
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
