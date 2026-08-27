namespace OrderService.DTOs
{
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
