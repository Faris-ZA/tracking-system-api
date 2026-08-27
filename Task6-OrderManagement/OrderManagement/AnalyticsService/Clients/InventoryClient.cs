using System.Net.Http.Json;
using AnalyticsService.DTOs;

namespace AnalyticsService.Clients
{
    public class InventoryClient : IInventoryClient
    {
        private readonly HttpClient _httpClient;

        public InventoryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            return await _httpClient
                .GetFromJsonAsync<List<ProductDto>>("products")
                ?? new List<ProductDto>();
        }
    }
}
