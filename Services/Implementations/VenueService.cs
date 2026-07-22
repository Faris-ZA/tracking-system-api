using WebApplication2.DTOs.Venues;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;

        public VenueService(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task<List<VenueResponseDto>> GetAllAsync()
        {
            var venues = await _venueRepository.GetAllAsync();

            return venues
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<VenueResponseDto?> GetByIdAsync(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);

            if (venue == null)
            {
                return null;
            }

            return MapToResponseDto(venue);
        }

        public async Task<VenueResponseDto> CreateAsync(CreateVenueDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Venue name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.City))
            {
                throw new ArgumentException("Venue city is required.");
            }

            var name = dto.Name.Trim();
            var city = dto.City.Trim();

            var duplicateExists =
                await _venueRepository.ActiveNameExistsAsync(name);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "An active venue with this name already exists.");
            }

            var currentTime = DateTime.UtcNow;

            var venue = new Venue
            {
                Name = name,
                City = city,
                UpdateStatus = UpdateStatus.New,
                CreateDate = currentTime,
                LastUpdate = currentTime
            };

            await _venueRepository.AddAsync(venue);
            await _venueRepository.SaveChangesAsync();

            return MapToResponseDto(venue);
        }

        public async Task<VenueResponseDto?> UpdateAsync(
            int id,
            UpdateVenueDto dto)
        {
            var venue = await _venueRepository.GetByIdAsync(id);

            if (venue == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Venue name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.City))
            {
                throw new ArgumentException("Venue city is required.");
            }

            var name = dto.Name.Trim();
            var city = dto.City.Trim();

            var duplicateExists =
                await _venueRepository.ActiveNameExistsAsync(
                    name,
                    excludeId: id);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "Another active venue with this name already exists.");
            }

            venue.Name = name;
            venue.City = city;
            venue.UpdateStatus = UpdateStatus.Updated;
            venue.LastUpdate = DateTime.UtcNow;

            await _venueRepository.SaveChangesAsync();

            return MapToResponseDto(venue);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);

            if (venue == null)
            {
                return false;
            }

            var hasActiveFloors =
                await _venueRepository.HasActiveFloorsAsync(id);

            if (hasActiveFloors)
            {
                throw new InvalidOperationException(
                    "The venue cannot be deleted because it contains active floors.");
            }

            venue.UpdateStatus = UpdateStatus.Deleted;
            venue.LastUpdate = DateTime.UtcNow;

            await _venueRepository.SaveChangesAsync();

            return true;
        }

        private static VenueResponseDto MapToResponseDto(Venue venue)
        {
            return new VenueResponseDto
            {
                Id = venue.Id,
                Name = venue.Name,
                City = venue.City,
                CreateDate = venue.CreateDate,
                LastUpdate = venue.LastUpdate
            };
        }
    }
}
