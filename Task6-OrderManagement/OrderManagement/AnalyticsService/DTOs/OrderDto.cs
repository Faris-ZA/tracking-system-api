namespace AnalyticsService.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
