using System.Net.Http.Json;

namespace KafkaOrderService.Clients
{
    public class InventoryClient : IInventoryClient
    {
        private readonly HttpClient _httpClient;

        public InventoryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InventoryReservationResponse?> ReserveAsync(
            int productId,
            int quantity)
        {
            var request = new
            {
                ProductId = productId,
                Quantity = quantity
            };

            var response = await _httpClient.PostAsJsonAsync(
                "inventory/reserve",
                request);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<InventoryReservationResponse>();
        }
    }
}

