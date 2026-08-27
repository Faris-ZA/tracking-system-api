using KafkaCustomerService.Models;

namespace KafkaCustomerService.DTOs
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public CustomerStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
