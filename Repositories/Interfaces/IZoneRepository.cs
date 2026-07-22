using WebApplication2.Models;

namespace WebApplication2.Repositories.Interfaces
{
    public interface IZoneRepository
    {
        Task<List<Zone>> GetAllAsync();

        Task<Zone?> GetByIdAsync(int id);

        Task<bool> ActiveNameExistsInFloorAsync(
            string name,
            int floorId,
            int? excludeId = null);

        Task AddAsync(Zone zone);

        Task RemovePolygonPointsAsync(int zoneId);

        Task SaveChangesAsync();
    }
}