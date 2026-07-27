using WebApplication2.DTOs.Venues;

namespace WebApplication2.Services.Interfaces
{
    public interface IVenueService
    {
        Task<List<VenueResponseDto>> GetAllAsync();

        Task<VenueResponseDto> GetByIdAsync(int id);

        Task<VenueResponseDto> CreateAsync(CreateVenueDto dto);

        Task<VenueResponseDto> UpdateAsync(
            int id,
            UpdateVenueDto dto);

        Task DeleteAsync(int id);
    }
}
