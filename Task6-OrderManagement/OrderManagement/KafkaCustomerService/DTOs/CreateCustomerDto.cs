using KafkaCustomerService.Models;

namespace KafkaCustomerService.DTOs
{
    public class CreateCustomerDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public CustomerStatus Status { get; set; }
    }
}
