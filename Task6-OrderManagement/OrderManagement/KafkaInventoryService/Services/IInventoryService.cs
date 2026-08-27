using KafkaInventoryService.DTOs;

namespace KafkaInventoryService.Services
{
    public interface IKafkaInventoryService
    {
        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<List<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto?> UpdateStockAsync(int id, UpdateStockDto dto);
        Task<ReserveInventoryResponseDto> ReserveAsync(ReserveInventoryDto dto);
    }
}

