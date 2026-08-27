namespace OrderService.Clients
{
    public interface IInventoryClient
    {
        Task<InventoryReservationResponse?> ReserveAsync(
            int productId,
            int quantity);
    }

    public class InventoryReservationResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
