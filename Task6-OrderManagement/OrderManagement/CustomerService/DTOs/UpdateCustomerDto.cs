using CustomerService.Models;

namespace CustomerService.DTOs
{
    public class UpdateCustomerDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public CustomerStatus Status { get; set; }
    }
}