using WebApplication2.DTOs.Zones;

namespace WebApplication2.Services.Interfaces
{
    public interface IZoneService
    {
        Task<List<ZoneResponseDto>> GetAllAsync();

        Task<ZoneResponseDto?> GetByIdAsync(int id);

        Task<ZoneResponseDto> CreateAsync(CreateZoneDto dto);

        Task<ZoneResponseDto?> UpdateAsync(
            int id,
            UpdateZoneDto dto);

        Task<bool> DeleteAsync(int id);
    }
}