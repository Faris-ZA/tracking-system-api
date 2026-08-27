namespace KafkaOrderService.Events
{
    public class CustomerEvent
    {
        public Guid EventId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int Status { get; set; }

        public DateTime OccurredAt { get; set; }
    }
}
