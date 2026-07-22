using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.People
{
    public class UpdatePersonDto
    {
        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}