using KafkaCustomerService.DTOs;
using KafkaCustomerService.Events;
using KafkaCustomerService.Kafka;
using KafkaCustomerService.Models;
using KafkaCustomerService.Repositories;

namespace KafkaCustomerService.Services
{
    public class KafkaCustomerService : IKafkaCustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly ICustomerEventProducer _eventProducer;

        public KafkaCustomerService(
            ICustomerRepository repository,
            ICustomerEventProducer eventProducer)
        {
            _repository = repository;
            _eventProducer = eventProducer;
        }

        public async Task<CustomerResponseDto?> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
                return null;

            return MapToResponse(customer);
        }

        public async Task<List<CustomerResponseDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();

            return customers
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                Status = dto.Status,
                CreatedDate = DateTime.UtcNow
            };

            var createdCustomer = await _repository.CreateAsync(customer);

            await _eventProducer.PublishAsync(new CustomerEvent
            {
                EventId = Guid.NewGuid(),
                EventType = "customer-created",
                CustomerId = createdCustomer.Id,
                Name = createdCustomer.Name,
                Email = createdCustomer.Email,
                Status = (int)createdCustomer.Status,
                OccurredAt = DateTime.UtcNow
            });

            return MapToResponse(createdCustomer);
        }

        public async Task<CustomerResponseDto?> UpdateAsync(
            int id,
            UpdateCustomerDto dto)
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
                return null;

            customer.Name = dto.Name;
            customer.Email = dto.Email;
            customer.Status = dto.Status;

            await _repository.UpdateAsync(customer);

            await _eventProducer.PublishAsync(new CustomerEvent
            {
                EventId = Guid.NewGuid(),
                EventType = "customer-updated",
                CustomerId = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Status = (int)customer.Status,
                OccurredAt = DateTime.UtcNow
            });

            return MapToResponse(customer);
        }

        private static CustomerResponseDto MapToResponse(Customer customer)
        {
            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Status = customer.Status,
                CreatedDate = customer.CreatedDate
            };
        }
    }
}