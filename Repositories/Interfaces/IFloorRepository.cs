using WebApplication2.Models;

namespace WebApplication2.Repositories.Interfaces
{
    public interface IFloorRepository
    {
        Task<List<Floor>> GetAllAsync();

        Task<Floor?> GetByIdAsync(int id);

        Task<bool> ActiveNameExistsInVenueAsync(
            string name,
            int venueId,
            int? excludeId = null);

        Task<bool> ActiveLevelExistsInVenueAsync(
            int level,
            int venueId,
            int? excludeId = null);

        Task<bool> HasActiveZonesAsync(int floorId);

        Task AddAsync(Floor floor);

        Task SaveChangesAsync();
    }
}