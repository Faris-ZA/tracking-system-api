using KafkaCustomerService.DTOs;

namespace KafkaCustomerService.Services
{
    public interface IKafkaCustomerService
    {
        Task<CustomerResponseDto?> GetByIdAsync(int id);
        Task<List<CustomerResponseDto>> GetAllAsync();
        Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerResponseDto?> UpdateAsync(int id, UpdateCustomerDto dto);
    }
}
