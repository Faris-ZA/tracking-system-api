using System.Net;
using System.Net.Http.Json;

namespace OrderService.Clients
{
    public class CustomerClient : ICustomerClient
    {
        private readonly HttpClient _httpClient;

        public CustomerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            var response = await _httpClient.GetAsync($"customers/{customerId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }
    }
}
