using AnalyticsService.DTOs;

namespace AnalyticsService.Clients
{
    public interface IOrderClient
    {
        Task<List<OrderDto>> GetAllAsync();
    }
}
