using WebApplication2.DTOs.People;
using WebApplication2.DTOs.Performance;

namespace WebApplication2.Services.Interfaces
{
    public interface IPersonService
    {
        Task<List<PersonResponseDto>> GetAllAsync();

        Task<PersonResponseDto> GetByIdAsync(int id);

        Task<PersonResponseDto> CreateAsync(
            CreatePersonDto dto);

        Task<PersonResponseDto> UpdateAsync(
            int id,
            UpdatePersonDto dto);

        Task DeleteAsync(int id);

        Task<PerformancePageResponseDto<PersonResponseDto>>
            GetDatabasePerformanceAsync(
                PeoplePerformanceQueryDto queryDto);

        Task<PerformancePageResponseDto<PersonResponseDto>>
            GetCachePerformanceAsync(
                PeoplePerformanceQueryDto queryDto);
        Task WarmCacheAsync(
            CancellationToken cancellationToken);
    }
}

