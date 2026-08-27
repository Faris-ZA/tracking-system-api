using OrderService.Clients;
using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly ICustomerClient _customerClient;
        private readonly IInventoryClient _inventoryClient;

        public OrderService(
            IOrderRepository repository,
            ICustomerClient customerClient,
            IInventoryClient inventoryClient)
        {
            _repository = repository;
            _customerClient = customerClient;
            _inventoryClient = inventoryClient;
        }

        public async Task<OrderResponseDto?> GetByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
                return null;

            return MapToResponse(order);
        }

        public async Task<List<OrderResponseDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();

            return orders
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<List<OrderResponseDto>> GetByCustomerIdAsync(int customerId)
        {
            var orders = await _repository.GetByCustomerIdAsync(customerId);

            return orders
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
        {
            var customer = await _customerClient
                .GetCustomerByIdAsync(dto.CustomerId);

            if (customer == null)
                throw new InvalidOperationException("Customer does not exist.");

            if (customer.Status != 0)
                throw new InvalidOperationException("Customer is not active.");

            var reservation = await _inventoryClient.ReserveAsync(
                dto.ProductId,
                dto.Quantity);

            var status = reservation?.Success == true
                ? OrderStatus.Confirmed
                : OrderStatus.Rejected;

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                TotalAmount = dto.TotalAmount,
                Status = status,
                CreatedDate = DateTime.UtcNow
            };

            var createdOrder = await _repository.CreateAsync(order);

            return MapToResponse(createdOrder);
        }

        private static OrderResponseDto MapToResponse(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedDate = order.CreatedDate
            };
        }
    }
}
