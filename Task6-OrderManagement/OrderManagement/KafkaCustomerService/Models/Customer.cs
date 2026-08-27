namespace KafkaCustomerService.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public CustomerStatus Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
