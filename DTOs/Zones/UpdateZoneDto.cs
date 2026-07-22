using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Zones
{
    public class UpdateZoneDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}