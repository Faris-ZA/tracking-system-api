using InventoryService.DTOs;

namespace InventoryService.Services
{
    public interface IInventoryService
    {
        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<List<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto?> UpdateStockAsync(int id, UpdateStockDto dto);
        Task<ReserveInventoryResponseDto> ReserveAsync(ReserveInventoryDto dto);
    }
}
