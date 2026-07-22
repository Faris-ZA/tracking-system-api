using WebApplication2.DTOs.People;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<List<PersonResponseDto>> GetAllAsync()
        {
            var people = await _personRepository.GetAllAsync();

            return people
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<PersonResponseDto?> GetByIdAsync(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);

            return person == null
                ? null
                : MapToResponseDto(person);
        }

        public async Task<PersonResponseDto> CreateAsync(
            CreatePersonDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException(
                    "Person name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                throw new ArgumentException(
                    "Phone number is required.");
            }

            var name = dto.Name.Trim();
            var phone = dto.Phone.Trim();

            var duplicateExists =
                await _personRepository.ActiveNameExistsAsync(name);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "An active person with this name already exists.");
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

        public async Task<PersonResponseDto?> UpdateAsync(
            int id,
            UpdatePersonDto dto)
        {
            var person = await _personRepository.GetByIdAsync(id);

            if (person == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                throw new ArgumentException(
                    "Phone number is required.");
            }

            person.Phone = dto.Phone.Trim();
            person.UpdateStatus = UpdateStatus.Updated;
            person.LastUpdate = DateTime.UtcNow;

            await _personRepository.SaveChangesAsync();

            return MapToResponseDto(person);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);

            if (person == null)
            {
                return false;
            }

            var hasAssociation =
                await _personRepository.HasTagAssociationAsync(id);

            if (hasAssociation)
            {
                throw new InvalidOperationException(
                    "The person cannot be deleted because they are associated with a tag.");
            }

            person.UpdateStatus = UpdateStatus.Deleted;
            person.LastUpdate = DateTime.UtcNow;

            await _personRepository.SaveChangesAsync();

            return true;
        }

        private static PersonResponseDto MapToResponseDto(
            Person person)
        {
            return new PersonResponseDto
            {
                Id = person.Id,
                Name = person.Name,
                Phone = person.Phone,
                CreateDate = person.CreateDate,
                LastUpdate = person.LastUpdate
            };
        }
    }
}
