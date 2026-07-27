using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.People
{
    public class CreatePersonDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}