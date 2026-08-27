using AnalyticsService.DTOs;

namespace AnalyticsService.Clients
{
    public interface ICustomerClient
    {
        Task<List<CustomerDto>> GetAllAsync();
    }
}
