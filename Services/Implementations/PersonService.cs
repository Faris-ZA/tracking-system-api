using WebApplication2.Constants;
using WebApplication2.DTOs.People;
using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;
using System.Text.RegularExpressions;

namespace WebApplication2.Services.Implementations
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        public PersonService(
            IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<List<PersonResponseDto>> GetAllAsync()
        {
            var people =
                await _personRepository.GetAllAsync();

            return people
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<PersonResponseDto> GetByIdAsync(
            int id)
        {
            var person =
                await _personRepository.GetByIdAsync(id);

            if (person == null)
            {
                throw new NotFoundException(
                    ErrorMessages.PersonNotFound);
            }

            return MapToResponseDto(person);
        }

        public async Task<PersonResponseDto> CreateAsync(
            CreatePersonDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException(
                    ErrorMessages.PersonNameRequired);
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                throw new BadRequestException(
                    ErrorMessages.PhoneNumberRequired);
            }

            var name = dto.Name.Trim();
            var phone = dto.Phone.Trim();

            ValidatePhoneNumber(phone);

            var duplicateExists =
                await _personRepository
                    .ActiveNameExistsAsync(name);

            if (duplicateExists)
            {
                throw new ConflictException(
                    ErrorMessages.PersonNameAlreadyExists);
            }

            var currentTime = DateTime.UtcNow;

            var person = new Person
            {
                Name = name,
                Phone = phone,
                UpdateStatus = UpdateStatus.New,
                CreateDate = currentTime,
                LastUpdate = currentTime
            };

            await _personRepository.AddAsync(person);
            await _personRepository.SaveChangesAsync();

            return MapToResponseDto(person);
        }

        public async Task<PersonResponseDto> UpdateAsync(
            int id,
            UpdatePersonDto dto)
        {
            var person =
                await _personRepository.GetByIdAsync(id);

            if (person == null)
            {
                throw new NotFoundException(
                    ErrorMessages.PersonNotFound);
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                throw new BadRequestException(
                    ErrorMessages.PhoneNumberRequired);
            }

            var phone = dto.Phone.Trim();

            ValidatePhoneNumber(phone);

            person.Phone = phone;

            person.UpdateStatus =
                UpdateStatus.Updated;
            person.LastUpdate = DateTime.UtcNow;

            await _personRepository.SaveChangesAsync();

            return MapToResponseDto(person);
        }

        public async Task DeleteAsync(int id)
        {
            var person =
                await _personRepository.GetByIdAsync(id);

            if (person == null)
            {
                throw new NotFoundException(
                    ErrorMessages.PersonNotFound);
            }

            var hasAssociation =
                await _personRepository
                    .HasTagAssociationAsync(id);

            if (hasAssociation)
            {
                throw new ConflictException(
                    ErrorMessages.PersonAssociatedWithTag);
            }

            person.UpdateStatus =
                UpdateStatus.Deleted;
            person.LastUpdate = DateTime.UtcNow;

            await _personRepository.SaveChangesAsync();
        }
        private static void ValidatePhoneNumber(string phone)
        {
            var isValid =
                Regex.IsMatch(phone, @"^\+?\d+$");

            if (!isValid)
            {
                throw new BadRequestException(
                    ErrorMessages.InvalidPhoneNumber);
            }
        }

        private static PersonResponseDto MapToResponseDto(
            Person person)
        {
            var activeAssociation = person.TagAssociations
                .FirstOrDefault(a =>
                a.Tag.UpdateStatus != UpdateStatus.Deleted);

            return new PersonResponseDto
            {
                Id = person.Id,
                Name = person.Name,
                Phone = person.Phone,
                CreateDate = person.CreateDate,
                LastUpdate = person.LastUpdate,

                AssociatedTag = activeAssociation == null 
                ? null
                : new AssociatedTagDto
                {
                    Id = activeAssociation.Tag.Id,
                    Mac = activeAssociation.Tag.Mac 
                }
            };
        }
    }
}







