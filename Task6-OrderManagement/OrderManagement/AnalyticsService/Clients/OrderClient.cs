using System.Net.Http.Json;
using AnalyticsService.DTOs;

namespace AnalyticsService.Clients
{
    public class OrderClient : IOrderClient
    {
        private readonly HttpClient _httpClient;

        public OrderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            return await _httpClient
                .GetFromJsonAsync<List<OrderDto>>("orders")
                ?? new List<OrderDto>();
        }
    }
}
