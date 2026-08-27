using CustomerService.DTOs;
using CustomerService.Models;
using CustomerService.Repositories;

namespace CustomerService.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
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