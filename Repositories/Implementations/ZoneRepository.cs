using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;

namespace WebApplication2.Repositories.Implementations
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly AppDbContext _context;

        public ZoneRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Zone>> GetAllAsync()
        {
            return await _context.Zones
                .Where(z => z.UpdateStatus != UpdateStatus.Deleted)
                .ToListAsync();
        }

        public async Task<Zone?> GetByIdAsync(int id)
        {
            return await _context.Zones
                .FirstOrDefaultAsync(z =>
                    z.Id == id &&
                    z.UpdateStatus != UpdateStatus.Deleted);
        }

        public async Task<bool> ActiveNameExistsInFloorAsync(
            string name,
            int floorId,
            int? excludeId = null)
        {
            var normalizedName = name.ToLower();

            return await _context.Zones.AnyAsync(z =>
                z.FloorId == floorId &&
                z.UpdateStatus != UpdateStatus.Deleted &&
                z.Name.ToLower() == normalizedName &&
                (!excludeId.HasValue || z.Id != excludeId.Value));
        }

        public async Task AddAsync(Zone zone)
        {
            await _context.Zones.AddAsync(zone);
        }

        public async Task RemovePolygonPointsAsync(int zoneId)
        {
            var points = await _context.ZonePolygonPoints
                .Where(p => p.ZoneId == zoneId)
                .ToListAsync();

            _context.ZonePolygonPoints.RemoveRange(points);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}