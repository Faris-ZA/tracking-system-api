using WebApplication2.Constants;
using WebApplication2.DTOs.Zones;
using WebApplication2.Exceptions;
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

        public async Task<ZoneResponseDto> GetByIdAsync(int id)
        {
            var zone = await _zoneRepository.GetByIdAsync(id);

            if (zone == null)
            {
                throw new NotFoundException(
                    ErrorMessages.ZoneNotFound);
            }

            return MapToResponseDto(zone);
        }

        public async Task<ZoneResponseDto> CreateAsync(
            CreateZoneDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException(
                    ErrorMessages.ZoneNameRequired);
            }

            ValidatePolygonPoints(dto.PolygonPoints);

            var floor =
                await _floorRepository.GetByIdAsync(dto.FloorId);

            if (floor == null)
            {
                throw new BadRequestException(
                    ErrorMessages.AssignedFloorNotFound);
            }

            var name = dto.Name.Trim();

            var duplicateExists =
                await _zoneRepository
                    .ActiveNameExistsInFloorAsync(
                        name,
                        dto.FloorId);

            if (duplicateExists)
            {
                throw new ConflictException(
                    ErrorMessages.ZoneNameAlreadyExistsOnFloor);
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

            for (var index = 0;
                index < dto.PolygonPoints.Count;
                index++)
            {
                var pointDto = dto.PolygonPoints[index];

                zone.PolygonPoints.Add(
                    new ZonePolygonPoint
                    {
                        PointIndex = index,
                        X = pointDto.X,
                        Y = pointDto.Y,
                        CreateDate = currentTime,
                        LastUpdate = currentTime
                    });
            }

            await _zoneRepository.AddAsync(zone);
            await _zoneRepository.SaveChangesAsync();

            return MapToResponseDto(zone);
        }

        public async Task<ZoneResponseDto> UpdateAsync(
            int id,
            UpdateZoneDto dto)
        {
            var zone = await _zoneRepository.GetByIdAsync(id);

            if (zone == null)
            {
                throw new NotFoundException(
                    ErrorMessages.ZoneNotFound);
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException(
                    ErrorMessages.ZoneNameRequired);
            }

            ValidatePolygonPoints(dto.PolygonPoints);

            var name = dto.Name.Trim();

            var duplicateExists =
                await _zoneRepository
                    .ActiveNameExistsInFloorAsync(
                        name,
                        zone.FloorId,
                        excludeId: id);

            if (duplicateExists)
            {
                throw new ConflictException(
                    ErrorMessages.AnotherZoneNameAlreadyExistsOnFloor);
            }

            var currentTime = DateTime.UtcNow;

            zone.Name = name;
            zone.UpdateStatus = UpdateStatus.Updated;
            zone.LastUpdate = DateTime.UtcNow;

            UpdatePolygonPoints(
                zone,
                dto.PolygonPoints,
                currentTime);

            await _zoneRepository.SaveChangesAsync();

            return MapToResponseDto(zone);
        }

        public async Task DeleteAsync(int id)
        {
            var zone = await _zoneRepository.GetByIdAsync(id);

            if (zone == null)
            {
                throw new NotFoundException(
                    ErrorMessages.ZoneNotFound);
            }

            await _zoneRepository.RemovePolygonPointsAsync(id);

            zone.UpdateStatus = UpdateStatus.Deleted;
            zone.LastUpdate = DateTime.UtcNow;

            await _zoneRepository.SaveChangesAsync();
        }

        private static void ValidatePolygonPoints(
            List<PolygonPointDto> polygonPoints)
        {
            if (polygonPoints.Count < 3)
            {
                throw new BadRequestException(
                    ErrorMessages.PolygonRequiresThreePoints);
            }

            var containsDuplicates = polygonPoints
                 .GroupBy(p => new
                 {
                     p.X,
                     p.Y
                 }).Any(g => g.Count() > 1);

            if (containsDuplicates)
            {
                throw new BadRequestException(
                    ErrorMessages.PolygonContainsDuplicateCoordinates);
            }
        }

        private static void UpdatePolygonPoints(
            Zone zone,
            List<PolygonPointDto> polygonPoints,
            DateTime currentTime)
        {
            var existingPoints = zone.PolygonPoints
                .OrderBy(point => point.PointIndex)
                .ToList();

            for (var index = 0;
                 index < polygonPoints.Count;
                 index++)
            {
                var pointDto = polygonPoints[index];

                var existingPoint = existingPoints
                    .FirstOrDefault(point =>
                        point.PointIndex == index);

                if (existingPoint == null)
                {
                    zone.PolygonPoints.Add(
                        new ZonePolygonPoint
                        {
                            ZoneId = zone.Id,
                            PointIndex = index,
                            X = pointDto.X,
                            Y = pointDto.Y,
                            CreateDate = currentTime,
                            LastUpdate = currentTime
                        });
                }
                else
                {
                    existingPoint.X = pointDto.X;
                    existingPoint.Y = pointDto.Y;
                    existingPoint.LastUpdate = currentTime;
                }
            }

            var extraPoints = existingPoints
                .Where(point =>
                    point.PointIndex >= polygonPoints.Count)
                .ToList();

            foreach (var extraPoint in extraPoints)
            {
                zone.PolygonPoints.Remove(extraPoint);
            }
        }

        private static ZoneResponseDto MapToResponseDto(
            Zone zone)
        {
            return new ZoneResponseDto
            {
                Id = zone.Id,
                Name = zone.Name,
                FloorId = zone.FloorId,
                CreateDate = zone.CreateDate,
                LastUpdate = zone.LastUpdate,

                PolygonPoints = zone.PolygonPoints
                    .OrderBy(point => point.PointIndex)
                    .Select(point =>
                       new PolygonPointResponseDto
                        {
                            PointIndex = point.PointIndex,
                            X = point.X,
                            Y = point.Y
                        })
                    .ToList()
            };
        }
    }
}







