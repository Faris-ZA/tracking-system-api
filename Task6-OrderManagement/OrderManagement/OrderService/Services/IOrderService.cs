using OrderService.DTOs;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto?> GetByIdAsync(int id);
        Task<List<OrderResponseDto>> GetAllAsync();
        Task<List<OrderResponseDto>> GetByCustomerIdAsync(int customerId);
        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
    }
}
