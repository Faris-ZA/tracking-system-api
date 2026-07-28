using WebApplication2.Constants;
using WebApplication2.DTO.Positions;
using WebApplication2.DTOs.Positions;
using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IZoneRepository _zoneRepository;

        public PositionService(
            IPositionRepository positionRepository,
            IPersonRepository personRepository,
            IVenueRepository venueRepository,
            IFloorRepository floorRepository,
            IZoneRepository zoneRepository)
        {
            _positionRepository = positionRepository;
            _personRepository = personRepository;
            _venueRepository = venueRepository;
            _floorRepository = floorRepository;
            _zoneRepository = zoneRepository;
        }

        public async Task<PositionResponseDto> CreateAsync(
            CreatePositionDto dto)
        {
            var person =
                await _personRepository.GetByIdAsync(dto.PersonId);

            if (person == null)
            {
                throw new NotFoundException(
                    ErrorMessages.PersonNotFound);
            }

            var venue =
                await _venueRepository.GetByIdAsync(dto.VenueId);

            if (venue == null)
            {
                throw new NotFoundException(
                    ErrorMessages.VenueNotFound);
            }

            var floor =
                await _floorRepository.GetByIdAsync(dto.FloorId);

            if (floor == null)
            {
                throw new NotFoundException(
                    ErrorMessages.FloorNotFound);
            }

            var zone =
                await _zoneRepository.GetByIdAsync(dto.ZoneId);

            if (zone == null)
            {
                throw new NotFoundException(
                    ErrorMessages.ZoneNotFound);
            }

            if (floor.VenueId != dto.VenueId)
            {
                throw new BadRequestException(
                    ErrorMessages.FloorDoesNotBelongToVenue);
            }

            if (zone.FloorId != dto.FloorId)
            {
                throw new BadRequestException(
                    ErrorMessages.ZoneDoesNotBelongToFloor);
            }

            var currentTime = DateTime.UtcNow;

            var lastPosition =
                await _positionRepository
                    .GetLastPositionByPersonIdAsync(dto.PersonId);

            if (lastPosition == null)
            {
                lastPosition = new LastPosition
                {
                    PeopleId = dto.PersonId,
                    VenueId = dto.VenueId,
                    FloorId = dto.FloorId,
                    ZoneId = dto.ZoneId,
                    X = dto.X,
                    Y = dto.Y,
                    CreateDate = currentTime,
                    LastUpdate = currentTime
                };

                await _positionRepository
                    .AddLastPositionAsync(lastPosition);
            }
            else
            {
                lastPosition.VenueId = dto.VenueId;
                lastPosition.FloorId = dto.FloorId;
                lastPosition.ZoneId = dto.ZoneId;
                lastPosition.X = dto.X;
                lastPosition.Y = dto.Y;
                lastPosition.LastUpdate = currentTime;
            }

            var history = new PositionHistory
            {
                PeopleId = dto.PersonId,
                VenueId = dto.VenueId,
                FloorId = dto.FloorId,
                ZoneId = dto.ZoneId,
                X = dto.X,
                Y = dto.Y,
                CreateDate = currentTime
            };

            await _positionRepository
                .AddPositionHistoryAsync(history);

            await _positionRepository.SaveChangesAsync();

            return new PositionResponseDto
            {
                PersonId = lastPosition.PeopleId,
                VenueId = lastPosition.VenueId,
                FloorId = lastPosition.FloorId,
                ZoneId = lastPosition.ZoneId,
                X = lastPosition.X,
                Y = lastPosition.Y,
                LastUpdate = lastPosition.LastUpdate
            };
        }
    }
}