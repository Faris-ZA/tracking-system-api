using CustomerService.DTOs;

namespace CustomerService.Services
{
    public interface ICustomerService
    {
        Task<CustomerResponseDto?> GetByIdAsync(int id);
        Task<List<CustomerResponseDto>> GetAllAsync();
        Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerResponseDto?> UpdateAsync(int id, UpdateCustomerDto dto);
    }
}