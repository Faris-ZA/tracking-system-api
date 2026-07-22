using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Tags
{
    public class CreateTagDto
    {
        [Required]
        public string Label { get; set; } = string.Empty;

        [Required]
        public string Mac { get; set; } = string.Empty;
    }
}
