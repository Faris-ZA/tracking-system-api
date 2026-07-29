using WebApplication2.Constants;
using WebApplication2.DTOs.Floors;
using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IVenueRepository _venueRepository;

        public FloorService(
            IFloorRepository floorRepository,
            IVenueRepository venueRepository)
        {
            _floorRepository = floorRepository;
            _venueRepository = venueRepository;
        }

        public async Task<List<FloorResponseDto>> GetAllAsync()
        {
            var floors = await _floorRepository.GetAllAsync();

            return floors
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<FloorResponseDto> GetByIdAsync(int id)
        {
            var floor = await _floorRepository.GetByIdAsync(id);

            if (floor == null)
            {
                throw new NotFoundException(
                    ErrorMessages.FloorNotFound);
            }

            return MapToResponseDto(floor);
        }

        public async Task<FloorResponseDto> CreateAsync(
            CreateFloorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException(
                    ErrorMessages.FloorNameRequired);
            }

            var venue =
                await _venueRepository.GetByIdAsync(dto.VenueId);

            if (venue == null)
            {
                throw new BadRequestException(
                    ErrorMessages.AssignedVenueNotFound);
            }

            var name = dto.Name.Trim();

            var duplicateName =
                await _floorRepository
                    .ActiveNameExistsInVenueAsync(
                        name,
                        dto.VenueId);

            if (duplicateName)
            {
                throw new ConflictException(
                    ErrorMessages.FloorNameAlreadyExistsInVenue);
            }

            var duplicateLevel =
                await _floorRepository
                    .ActiveLevelExistsInVenueAsync(
                        dto.Level,
                        dto.VenueId);

            if (duplicateLevel)
            {
                throw new ConflictException(
                    ErrorMessages.FloorLevelAlreadyExistsInVenue);
            }

            var currentTime = DateTime.UtcNow;

            var floor = new Floor
            {
                Name = name,
                VenueId = dto.VenueId,
                Level = dto.Level,
                UpdateStatus = UpdateStatus.New,
                CreateDate = currentTime,
                LastUpdate = currentTime
            };

            await _floorRepository.AddAsync(floor);
            await _floorRepository.SaveChangesAsync();

            return MapToResponseDto(floor);
        }

        public async Task<FloorResponseDto> UpdateAsync(
            int id,
            UpdateFloorDto dto)
        {
            var floor = await _floorRepository.GetByIdAsync(id);

            if (floor == null)
            {
                throw new NotFoundException(
                    ErrorMessages.FloorNotFound);
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException(
                    ErrorMessages.FloorNameRequired);
            }

            var name = dto.Name.Trim();

            var duplicateName =
                await _floorRepository
                    .ActiveNameExistsInVenueAsync(
                        name,
                        floor.VenueId,
                        excludeId: id);

            if (duplicateName)
            {
                throw new ConflictException(
                    ErrorMessages.AnotherFloorNameAlreadyExistsInVenue);
            }

            var duplicateLevel =
                await _floorRepository
                    .ActiveLevelExistsInVenueAsync(
                        dto.Level,
                        floor.VenueId,
                        excludeId: id);

            if (duplicateLevel)
            {
                throw new ConflictException(
                    ErrorMessages.AnotherFloorLevelAlreadyExistsInVenue);
            }

            floor.Name = name;
            floor.Level = dto.Level;
            floor.UpdateStatus = UpdateStatus.Updated;
            floor.LastUpdate = DateTime.UtcNow;

            await _floorRepository.SaveChangesAsync();

            return MapToResponseDto(floor);
        }

        public async Task DeleteAsync(int id)
        {
            var floor = await _floorRepository.GetByIdAsync(id);

            if (floor == null)
            {
                throw new NotFoundException(
                    ErrorMessages.FloorNotFound);
            }

            var hasActiveZones =
                await _floorRepository.HasActiveZonesAsync(id);

            if (hasActiveZones)
            {
                throw new ConflictException(
                    ErrorMessages.FloorHasActiveZones);
            }

            floor.UpdateStatus = UpdateStatus.Deleted;
            floor.LastUpdate = DateTime.UtcNow;

            await _floorRepository.SaveChangesAsync();
        }

        private static FloorResponseDto MapToResponseDto(
            Floor floor)
        {
            return new FloorResponseDto
            {
                Id = floor.Id,
                Name = floor.Name,
                VenueId = floor.VenueId,
                Level = floor.Level,
                CreateDate = floor.CreateDate,
                LastUpdate = floor.LastUpdate
            };
        }
    }
}










