namespace KafkaOrderService.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}

