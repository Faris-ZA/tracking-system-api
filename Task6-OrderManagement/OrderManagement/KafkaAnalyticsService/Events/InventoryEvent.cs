namespace KafkaAnalyticsService.Events
{
    public class InventoryEvent
    {
        public Guid EventId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime OccurredAt { get; set; }
    }
}
