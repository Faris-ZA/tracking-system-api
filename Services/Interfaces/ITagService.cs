using WebApplication2.DTOs.Tags;

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
    }
}
