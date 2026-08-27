using AnalyticsService.DTOs;

namespace AnalyticsService.Clients
{
    public interface IInventoryClient
    {
        Task<List<ProductDto>> GetAllAsync();
    }
}
