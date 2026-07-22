using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs.People;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PeopleController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var people = await _personService.GetAllAsync();

            return Ok(people);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _personService.GetByIdAsync(id);

            if (person == null)
            {
                return NotFound(new
                {
                    message = "Person not found."
                });
            }

            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePersonDto dto)
        {
            try
            {
                var person =
                    await _personService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = person.Id },
                    person);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdatePersonDto dto)
        {
            try
            {
                var person =
                    await _personService.UpdateAsync(id, dto);

                if (person == null)
                {
                    return NotFound(new
                    {
                        message = "Person not found."
                    });
                }

                return Ok(person);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted =
                    await _personService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Person not found."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}