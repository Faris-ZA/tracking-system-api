using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Tags
{
    public class UpdateTagDto
    {
        [Required]
        public string Label { get; set; } = string.Empty;
    }
}
