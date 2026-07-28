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

        public PeopleTagAssociationService(
            IPeopleTagAssociationRepository associationRepository,
            IPersonRepository personRepository,
            ITagRepository tagRepository)
        {
            _associationRepository = associationRepository;
            _personRepository = personRepository;
            _tagRepository = tagRepository;
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

            var tag =
                await _tagRepository.GetByIdAsync(dto.TagId);

            if (tag == null)
            {
                throw new NotFoundException(
                    ErrorMessages.TagNotFound);
            }

            var personHasAssociation =
                await _personRepository
                    .HasTagAssociationAsync(dto.PersonId);

            if (personHasAssociation)
            {
                throw new ConflictException(
                    ErrorMessages.PersonAlreadyAssociated);
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

            var tag =
                await _tagRepository.GetByIdAsync(tagId);

            if (tag == null)
            {
                throw new NotFoundException(
                    ErrorMessages.TagNotFound);
            }

            var association =
                await _associationRepository.GetAsync(
                    personId,
                    tagId);

            if (association == null)
            {
                throw new NotFoundException(
                    ErrorMessages.AssociationNotFound);
            }

            _associationRepository.Remove(association);

            await _associationRepository.SaveChangesAsync();
        }

    }
}