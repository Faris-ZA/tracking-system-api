using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;

namespace WebApplication2.Repositories.Implementations
{
    public class FloorRepository : IFloorRepository
    {
        private readonly AppDbContext _context;

        public FloorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Floor>> GetAllAsync()
        {
            return await _context.Floors
                .Where(f => f.UpdateStatus != UpdateStatus.Deleted)
                .ToListAsync();
        }

        public async Task<Floor?> GetByIdAsync(int id)
        {
            return await _context.Floors
                .FirstOrDefaultAsync(f =>
                    f.Id == id &&
                    f.UpdateStatus != UpdateStatus.Deleted);
        }

        public async Task<bool> ActiveNameExistsInVenueAsync(
            string name,
            int venueId,
            int? excludeId = null)
        {
            var normalizedName = name.ToLower();

            return await _context.Floors.AnyAsync(f =>
                f.VenueId == venueId &&
                f.UpdateStatus != UpdateStatus.Deleted &&
                f.Name.ToLower() == normalizedName &&
                (!excludeId.HasValue || f.Id != excludeId.Value));
        }

        public async Task<bool> ActiveLevelExistsInVenueAsync(
            int level,
            int venueId,
            int? excludeId = null)
        {
            return await _context.Floors.AnyAsync(f =>
                f.VenueId == venueId &&
                f.Level == level &&
                f.UpdateStatus != UpdateStatus.Deleted &&
                (!excludeId.HasValue || f.Id != excludeId.Value));
        }

        public async Task<bool> HasActiveZonesAsync(int floorId)
        {
            return await _context.Zones.AnyAsync(z =>
                z.FloorId == floorId &&
                z.UpdateStatus != UpdateStatus.Deleted);
        }

        public async Task AddAsync(Floor floor)
        {
            await _context.Floors.AddAsync(floor);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}