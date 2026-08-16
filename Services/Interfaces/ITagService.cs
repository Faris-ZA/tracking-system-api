using WebApplication2.DTOs.Tags;
using WebApplication2.DTOs.Performance;

namespace WebApplication2.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<TagResponseDto>> GetAllAsync();

        Task<TagResponseDto> GetByIdAsync(int id);

        Task<TagResponseDto> CreateAsync(
            CreateTagDto dto);

        Task<TagResponseDto> UpdateAsync(
            int id,
            UpdateTagDto dto);

        Task DeleteAsync(int id);

        Task<PerformancePageResponseDto<TagResponseDto>>
            GetDatabasePerformanceAsync(
                TagPerformanceQueryDto queryDto);

        Task<PerformancePageResponseDto<TagResponseDto>>
            GetCachePerformanceAsync(
                TagPerformanceQueryDto queryDto);

        Task WarmCacheAsync(
            CancellationToken cancellationToken);
    }
}
