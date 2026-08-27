using KafkaOrderService.DTOs;

namespace KafkaOrderService.Services
{
    public interface IKafkaOrderService
    {
        Task<OrderResponseDto?> GetByIdAsync(int id);
        Task<List<OrderResponseDto>> GetAllAsync();
        Task<List<OrderResponseDto>> GetByCustomerIdAsync(int customerId);
        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
    }
}

