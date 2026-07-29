using Microsoft.AspNetCore.Mvc;
using WebApplication2.Dtos.PeopleTagAssociations;
using WebApplication2.DTOs.PeopleTagAssociations;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleTagAssociationsController : ControllerBase
    {
        private readonly IPeopleTagAssociationService
            _associationService;

        public PeopleTagAssociationsController(
            IPeopleTagAssociationService associationService)
        {
            _associationService = associationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePeopleTagAssociationDto dto)
        {
            var result =
                await _associationService.CreateAsync(dto);

            return Ok(result);
        }

        [HttpDelete("{personId:int}/{tagId:int}")]
        public async Task<IActionResult> Delete(
        int personId,
        int tagId)
        {
            await _associationService.DeleteAsync(
                personId,
                tagId);

            return NoContent();
        }
    }
}