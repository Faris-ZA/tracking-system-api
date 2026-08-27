using System.Net.Http.Json;
using AnalyticsService.DTOs;

namespace AnalyticsService.Clients
{
    public class CustomerClient : ICustomerClient
    {
        private readonly HttpClient _httpClient;

        public CustomerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            return await _httpClient
                .GetFromJsonAsync<List<CustomerDto>>("customers")
                ?? new List<CustomerDto>();
        }
    }
}
