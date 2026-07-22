using WebApplication2.DTOs.Tags;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagResponseDto>> GetAllAsync()
        {
            var tags = await _tagRepository.GetAllAsync();

            return tags
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<TagResponseDto?> GetByIdAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);

            return tag == null
                ? null
                : MapToResponseDto(tag);
        }

        public async Task<TagResponseDto> CreateAsync(
            CreateTagDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Label))
            {
                throw new ArgumentException(
                    "Tag label is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Mac))
            {
                throw new ArgumentException(
                    "Tag MAC address is required.");
            }

            var label = dto.Label.Trim();
            var mac = dto.Mac.Trim();

            var duplicateExists =
                await _tagRepository.ActiveMacExistsAsync(mac);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "An active tag with this MAC address already exists.");
            }

            var currentTime = DateTime.UtcNow;

            var tag = new Tag
            {
                Label = label,
                Mac = mac,
                UpdateStatus = UpdateStatus.New,
                CreateDate = currentTime,
                LastUpdate = currentTime
            };

            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync();

            return MapToResponseDto(tag);
        }

        public async Task<TagResponseDto?> UpdateAsync(
            int id,
            UpdateTagDto dto)
        {
            var tag = await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Label))
            {
                throw new ArgumentException(
                    "Tag label is required.");
            }

            tag.Label = dto.Label.Trim();
            tag.UpdateStatus = UpdateStatus.Updated;
            tag.LastUpdate = DateTime.UtcNow;

            await _tagRepository.SaveChangesAsync();

            return MapToResponseDto(tag);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                return false;
            }

            var hasAssociation =
                await _tagRepository.HasPeopleAssociationAsync(id);

            if (hasAssociation)
            {
                throw new InvalidOperationException(
                    "The tag cannot be deleted because it is associated with a person.");
            }

            tag.UpdateStatus = UpdateStatus.Deleted;
            tag.LastUpdate = DateTime.UtcNow;

            await _tagRepository.SaveChangesAsync();

            return true;
        }

        private static TagResponseDto MapToResponseDto(Tag tag)
        {
            return new TagResponseDto
            {
                Id = tag.Id,
                Label = tag.Label,
                Mac = tag.Mac,
                CreateDate = tag.CreateDate,
                LastUpdate = tag.LastUpdate
            };
        }
    }
}

