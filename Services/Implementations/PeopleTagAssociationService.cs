using WebApplication2.Services.Caching.Interfaces;
using WebApplication2.DTOs.Tags;
using WebApplication2.DTOs.People;
using WebApplication2.Constants;
using WebApplication2.Dtos.PeopleTagAssociations;
using WebApplication2.DTOs.PeopleTagAssociations;
using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class PeopleTagAssociationService
        : IPeopleTagAssociationService
    {
        private readonly IPeopleTagAssociationRepository
            _associationRepository;

        private readonly IPersonRepository
            _personRepository;

        private readonly ITagRepository
        _tagRepository;

    private readonly IPeopleCacheService
        _peopleCacheService;

    private readonly ITagCacheService
        _tagCacheService;

        public PeopleTagAssociationService(
        IPeopleTagAssociationRepository associationRepository,
        IPersonRepository personRepository,
        ITagRepository tagRepository,
        IPeopleCacheService peopleCacheService,
        ITagCacheService tagCacheService)
    {
        _associationRepository = associationRepository;
        _personRepository = personRepository;
        _tagRepository = tagRepository;
        _peopleCacheService = peopleCacheService;
        _tagCacheService = tagCacheService;
    }

        public async Task<PeopleTagAssociationResponseDto> CreateAsync(
            CreatePeopleTagAssociationDto dto)
        {
            var person =
                await _personRepository.GetByIdAsync(dto.PersonId);

            if (person == null)
            {
                throw new NotFoundException(
                    ErrorMessages.PersonNotFound);
            }

            var personHasAssociation =
               await _personRepository
                   .HasTagAssociationAsync(dto.PersonId);

            if (personHasAssociation)
            {
                throw new ConflictException(
                    ErrorMessages.PersonAlreadyAssociated);
            }

            var tag =
                await _tagRepository.GetByIdAsync(dto.TagId);

            if (tag == null)
            {
                throw new NotFoundException(
                    ErrorMessages.TagNotFound);
            }

            var tagHasAssociation =
                await _tagRepository
                    .HasPeopleAssociationAsync(dto.TagId);

            if (tagHasAssociation)
            {
                throw new ConflictException(
                    ErrorMessages.TagAlreadyAssociated);
            }

            var association = new PeopleTagAssociation
            {
                PeopleId = dto.PersonId,
                TagId = dto.TagId,
                CreateDate = DateTime.UtcNow
            };

            await _associationRepository.AddAsync(association);
        await _associationRepository.SaveChangesAsync();

        try
        {
            var personCache = new PersonResponseDto
            {
                Id = person.Id,
                Name = person.Name,
                Phone = person.Phone,
                CreateDate = person.CreateDate,
                LastUpdate = person.LastUpdate,
                AssociatedTag = new AssociatedTagDto
                {
                    Id = tag.Id,
                    Mac = tag.Mac
                }
            };

            var tagCache = new TagResponseDto
            {
                Id = tag.Id,
                Label = tag.Label,
                Mac = tag.Mac,
                CreateDate = tag.CreateDate,
                LastUpdate = tag.LastUpdate,
                AssociatedPerson = new AssociatedPersonDto
                {
                    Id = person.Id,
                    Name = person.Name
                }
            };

            await _peopleCacheService.SetAsync(personCache);
            await _tagCacheService.SetAsync(tagCache);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"{CacheMessages.AssociationCreateCacheFailed} {ex.Message}");

            try
            {
                await _peopleCacheService.RemoveAsync(person.Id);
                await _tagCacheService.RemoveAsync(tag.Id);
            }
            catch
            {
            }
        }

            return new PeopleTagAssociationResponseDto
            {
                PersonId = association.PeopleId,
                TagId = association.TagId,
                CreateDate = association.CreateDate
            };
        }
        public async Task DeleteAsync(
            int personId,
    int tagId)
        {
            var person =
                await _personRepository.GetByIdAsync(personId);

            if (person == null)
            {
                throw new NotFoundException(
                    ErrorMessages.PersonNotFound);
            }

            var association =
                await _associationRepository
                    .GetByPersonIdAsync(personId);

            if (association == null ||
                association.TagId != tagId)
            {
                throw new NotFoundException(
                    ErrorMessages.AssociationNotFound);
            }

            var tag =
                await _tagRepository.GetByIdAsync(tagId);

            if (tag == null)
            {
                throw new NotFoundException(
                    ErrorMessages.TagNotFound);
            }

            _associationRepository.Remove(association);

        await _associationRepository.SaveChangesAsync();

        try
        {
            var personCache = new PersonResponseDto
            {
                Id = person.Id,
                Name = person.Name,
                Phone = person.Phone,
                CreateDate = person.CreateDate,
                LastUpdate = person.LastUpdate,
                AssociatedTag = null
            };

            var tagCache = new TagResponseDto
            {
                Id = tag.Id,
                Label = tag.Label,
                Mac = tag.Mac,
                CreateDate = tag.CreateDate,
                LastUpdate = tag.LastUpdate,
                AssociatedPerson = null
            };

            await _peopleCacheService.SetAsync(personCache);
            await _tagCacheService.SetAsync(tagCache);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"{CacheMessages.AssociationDeleteCacheFailed} {ex.Message}");

            try
            {
                await _peopleCacheService.RemoveAsync(person.Id);
                await _tagCacheService.RemoveAsync(tag.Id);
            }
            catch
            {
            }
        }
        }

    }
}


