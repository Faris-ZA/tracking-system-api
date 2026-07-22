using WebApplication2.DTOs.Zones;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly IFloorRepository _floorRepository;

        public ZoneService(
            IZoneRepository zoneRepository,
            IFloorRepository floorRepository)
        {
            _zoneRepository = zoneRepository;
            _floorRepository = floorRepository;
        }

        public async Task<List<ZoneResponseDto>> GetAllAsync()
        {
            var zones = await _zoneRepository.GetAllAsync();

            return zones
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<ZoneResponseDto?> GetByIdAsync(int id)
        {
            var zone = await _zoneRepository.GetByIdAsync(id);

            return zone == null
                ? null
                : MapToResponseDto(zone);
        }

        public async Task<ZoneResponseDto> CreateAsync(
            CreateZoneDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException(
                    "Zone name is required.");
            }

            var floor =
                await _floorRepository.GetByIdAsync(dto.FloorId);

            if (floor == null)
            {
                throw new ArgumentException(
                    "The assigned floor does not exist.");
            }

            var name = dto.Name.Trim();

            var duplicateExists =
                await _zoneRepository
                    .ActiveNameExistsInFloorAsync(
                        name,
                        dto.FloorId);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "An active zone with this name already exists on the floor.");
            }

            var currentTime = DateTime.UtcNow;

            var zone = new Zone
            {
                Name = name,
                FloorId = dto.FloorId,
                UpdateStatus = UpdateStatus.New,
                CreateDate = currentTime,
                LastUpdate = currentTime
            };

            await _zoneRepository.AddAsync(zone);
            await _zoneRepository.SaveChangesAsync();

            return MapToResponseDto(zone);
        }

        public async Task<ZoneResponseDto?> UpdateAsync(
            int id,
            UpdateZoneDto dto)
        {
            var zone = await _zoneRepository.GetByIdAsync(id);

            if (zone == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException(
                    "Zone name is required.");
            }

            var name = dto.Name.Trim();

            var duplicateExists =
                await _zoneRepository
                    .ActiveNameExistsInFloorAsync(
                        name,
                        zone.FloorId,
                        excludeId: id);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "Another active zone with this name already exists on the floor.");
            }

            zone.Name = name;
            zone.UpdateStatus = UpdateStatus.Updated;
            zone.LastUpdate = DateTime.UtcNow;

            await _zoneRepository.SaveChangesAsync();

            return MapToResponseDto(zone);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var zone = await _zoneRepository.GetByIdAsync(id);

            if (zone == null)
            {
                return false;
            }

            await _zoneRepository.RemovePolygonPointsAsync(id);

            zone.UpdateStatus = UpdateStatus.Deleted;
            zone.LastUpdate = DateTime.UtcNow;

            await _zoneRepository.SaveChangesAsync();

            return true;
        }

        private static ZoneResponseDto MapToResponseDto(Zone zone)
        {
            return new ZoneResponseDto
            {
                Id = zone.Id,
                Name = zone.Name,
                FloorId = zone.FloorId,
                CreateDate = zone.CreateDate,
                LastUpdate = zone.LastUpdate
            };
        }
    }
}
