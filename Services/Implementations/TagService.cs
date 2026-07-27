using WebApplication2.Constants;
using WebApplication2.DTOs.Tags;
using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(
            ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagResponseDto>> GetAllAsync()
        {
            var tags =
                await _tagRepository.GetAllAsync();

            return tags
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<TagResponseDto> GetByIdAsync(
            int id)
        {
            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                throw new NotFoundException(
                    string.Format(
                        ErrorMessages.NotFound,
                        "Tag"));
            }

            return MapToResponseDto(tag);
        }

        public async Task<TagResponseDto> CreateAsync(
            CreateTagDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Label))
            {
                throw new BadRequestException(
                    string.Format(
                        ErrorMessages.Required,
                        "Tag label"));
            }

            if (string.IsNullOrWhiteSpace(dto.Mac))
            {
                throw new BadRequestException(
                    string.Format(
                        ErrorMessages.Required,
                        "Tag MAC address"));
            }

            var label = dto.Label.Trim();
            var mac = dto.Mac.Trim();

            var duplicateExists =
                await _tagRepository
                    .ActiveMacExistsAsync(mac);

            if (duplicateExists)
            {
                throw new ConflictException(
                    string.Format(
                        ErrorMessages.ActiveDuplicate,
                        "tag",
                        "MAC address",
                        ""));
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

        public async Task<TagResponseDto> UpdateAsync(
            int id,
            UpdateTagDto dto)
        {
            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                throw new NotFoundException(
                    string.Format(
                        ErrorMessages.NotFound,
                        "Tag"));
            }

            if (string.IsNullOrWhiteSpace(dto.Label))
            {
                throw new BadRequestException(
                    string.Format(
                        ErrorMessages.Required,
                        "Tag label"));
            }

            tag.Label = dto.Label.Trim();
            tag.UpdateStatus =
                UpdateStatus.Updated;
            tag.LastUpdate = DateTime.UtcNow;

            await _tagRepository.SaveChangesAsync();

            return MapToResponseDto(tag);
        }

        public async Task DeleteAsync(int id)
        {
            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                throw new NotFoundException(
                    string.Format(
                        ErrorMessages.NotFound,
                        "Tag"));
            }

            var hasAssociation =
                await _tagRepository
                    .HasPeopleAssociationAsync(id);

            if (hasAssociation)
            {
                throw new ConflictException(
                    string.Format(
                        ErrorMessages.CannotDeleteBecauseAssociated, 
                        "tag",
                        "person"));
            }

            tag.UpdateStatus =
                UpdateStatus.Deleted;
            tag.LastUpdate = DateTime.UtcNow;

            await _tagRepository.SaveChangesAsync();
        }

        private static TagResponseDto MapToResponseDto(
            Tag tag)
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


