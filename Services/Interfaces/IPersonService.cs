using WebApplication2.DTOs.People;

namespace WebApplication2.Services.Interfaces
{
    public interface IPersonService
    {
        Task<List<PersonResponseDto>> GetAllAsync();

        Task<PersonResponseDto?> GetByIdAsync(int id);

        Task<PersonResponseDto> CreateAsync(CreatePersonDto dto);

        Task<PersonResponseDto?> UpdateAsync(
            int id,
            UpdatePersonDto dto);

        Task<bool> DeleteAsync(int id);
    }
}