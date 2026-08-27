using KafkaOrderService.DTOs;
using KafkaOrderService.Events;
using KafkaOrderService.Kafka;
using KafkaOrderService.Models;
using KafkaOrderService.Repositories;

namespace KafkaOrderService.Services
{
    public class KafkaOrderService : IKafkaOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly ILocalCustomerRepository _localCustomerRepository;
        private readonly IOrderEventProducer _orderEventProducer;

        public KafkaOrderService(
            IOrderRepository repository,
            ILocalCustomerRepository localCustomerRepository,
            IOrderEventProducer orderEventProducer)
        {
            _repository = repository;
            _localCustomerRepository = localCustomerRepository;
            _orderEventProducer = orderEventProducer;
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
            var customer = await _localCustomerRepository
                .GetByIdAsync(dto.CustomerId);

            if (customer == null)
                throw new InvalidOperationException("Customer does not exist.");

            if (customer.Status != 0)
                throw new InvalidOperationException("Customer is not active.");

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                TotalAmount = dto.TotalAmount,
                Status = OrderStatus.Created,
                CreatedDate = DateTime.UtcNow
            };

            var createdOrder = await _repository.CreateAsync(order);

            await _orderEventProducer.PublishAsync(new OrderEvent
            {
                EventId = Guid.NewGuid(),
                EventType = "order-created",
                OrderId = createdOrder.OrderId,
                CustomerId = createdOrder.CustomerId,
                ProductId = createdOrder.ProductId,
                Quantity = createdOrder.Quantity,
                TotalAmount = createdOrder.TotalAmount,
                OccurredAt = DateTime.UtcNow
            });

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
