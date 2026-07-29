using WebApplication2.DTO.Positions;
using WebApplication2.DTOs.Positions;

namespace WebApplication2.Services.Interfaces
{
    public interface IPositionService
    {
        Task<PositionResponseDto> CreateAsync(
            CreatePositionDto dto);
    }
}