namespace KafkaInventoryService.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int AvailableQuantity { get; set; }

        public decimal Price { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}

