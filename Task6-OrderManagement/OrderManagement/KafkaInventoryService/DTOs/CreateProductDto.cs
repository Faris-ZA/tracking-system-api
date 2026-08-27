namespace KafkaInventoryService.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public int AvailableQuantity { get; set; }

        public decimal Price { get; set; }
    }
}

