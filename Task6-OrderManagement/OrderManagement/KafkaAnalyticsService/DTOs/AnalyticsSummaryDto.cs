namespace KafkaAnalyticsService.DTOs
{
    public class AnalyticsSummaryDto
    {
        public int TotalCustomers { get; set; }

        public int ActiveCustomers { get; set; }

        public int InactiveCustomers { get; set; }

        public int TotalOrders { get; set; }

        public int CreatedOrders { get; set; }

        public int ConfirmedOrders { get; set; }

        public int RejectedOrders { get; set; }

        public int InventoryReservedEvents { get; set; }

        public int InventoryRejectedEvents { get; set; }

        public int TotalReservedQuantity { get; set; }
    }
}
