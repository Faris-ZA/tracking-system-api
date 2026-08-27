using AnalyticsService.Clients;
using AnalyticsService.DTOs;

namespace AnalyticsService.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ICustomerClient _customerClient;
        private readonly IOrderClient _orderClient;
        private readonly IInventoryClient _inventoryClient;

        public AnalyticsService(
            ICustomerClient customerClient,
            IOrderClient orderClient,
            IInventoryClient inventoryClient)
        {
            _customerClient = customerClient;
            _orderClient = orderClient;
            _inventoryClient = inventoryClient;
        }

        public async Task<AnalyticsSummaryDto> GetSummaryAsync()
        {
            var customersTask = _customerClient.GetAllAsync();
            var ordersTask = _orderClient.GetAllAsync();
            var productsTask = _inventoryClient.GetAllAsync();

            await Task.WhenAll(
                customersTask,
                ordersTask,
                productsTask);

            var customers = await customersTask;
            var orders = await ordersTask;
            var products = await productsTask;

            return new AnalyticsSummaryDto
            {
                TotalCustomers = customers.Count,

                // CustomerStatus:
                // 0 = Active
                // 1 = Inactive
                ActiveCustomers =
                    customers.Count(c => c.Status == 0),

                InactiveCustomers =
                    customers.Count(c => c.Status == 1),

                TotalOrders = orders.Count,

                // OrderStatus:
                // 0 = Created
                // 1 = Confirmed
                // 2 = Rejected
                CreatedOrders =
                    orders.Count(o => o.Status == 0),

                ConfirmedOrders =
                    orders.Count(o => o.Status == 1),

                RejectedOrders =
                    orders.Count(o => o.Status == 2),

                TotalProducts = products.Count,

                TotalAvailableStock =
                    products.Sum(p => p.AvailableQuantity)
            };
        }
    }
}
