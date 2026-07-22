using WebApplication2.DTOs.Floors;

namespace WebApplication2.Services.Interfaces
{
    public interface IFloorService
    {
        Task<List<FloorResponseDto>> GetAllAsync();

        Task<FloorResponseDto?> GetByIdAsync(int id);

        Task<FloorResponseDto> CreateAsync(CreateFloorDto dto);

        Task<FloorResponseDto?> UpdateAsync(
            int id,
            UpdateFloorDto dto);

        Task<bool> DeleteAsync(int id);
    }
}