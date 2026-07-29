using WebApplication2.Dtos.PeopleTagAssociations;
using WebApplication2.DTOs.PeopleTagAssociations;

namespace WebApplication2.Services.Interfaces
{
    public interface IPeopleTagAssociationService
    {
        Task<PeopleTagAssociationResponseDto> CreateAsync(
            CreatePeopleTagAssociationDto dto);
        Task DeleteAsync(
            int personId,
            int tagId);
    }
}