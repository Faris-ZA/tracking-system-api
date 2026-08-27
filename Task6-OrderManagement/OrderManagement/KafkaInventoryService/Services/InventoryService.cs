using KafkaInventoryService.DTOs;
using KafkaInventoryService.Models;
using KafkaInventoryService.Repositories;

namespace KafkaInventoryService.Services
{
    public class KafkaInventoryService : IKafkaInventoryService
    {
        private readonly IInventoryRepository _repository;

        public KafkaInventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return null;

            return MapToResponse(product);
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();

            return products
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                AvailableQuantity = dto.AvailableQuantity,
                Price = dto.Price,
                UpdatedDate = DateTime.UtcNow
            };

            var createdProduct = await _repository.CreateAsync(product);

            return MapToResponse(createdProduct);
        }

        public async Task<ProductResponseDto?> UpdateStockAsync(
            int id,
            UpdateStockDto dto)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return null;

            product.AvailableQuantity = dto.AvailableQuantity;
            product.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(product);

            return MapToResponse(product);
        }

        public async Task<ReserveInventoryResponseDto> ReserveAsync(
            ReserveInventoryDto dto)
        {
            var product = await _repository.GetByIdAsync(dto.ProductId);

            if (product == null)
            {
                return new ReserveInventoryResponseDto
                {
                    Success = false,
                    Message = "Product does not exist."
                };
            }

            if (dto.Quantity <= 0)
            {
                return new ReserveInventoryResponseDto
                {
                    Success = false,
                    Message = "Quantity must be greater than zero."
                };
            }

            if (product.AvailableQuantity < dto.Quantity)
            {
                return new ReserveInventoryResponseDto
                {
                    Success = false,
                    Message = "Insufficient stock."
                };
            }

            product.AvailableQuantity -= dto.Quantity;
            product.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(product);

            return new ReserveInventoryResponseDto
            {
                Success = true,
                Message = "Inventory reserved successfully."
            };
        }

        private static ProductResponseDto MapToResponse(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                AvailableQuantity = product.AvailableQuantity,
                Price = product.Price,
                UpdatedDate = product.UpdatedDate
            };
        }
    }
}

