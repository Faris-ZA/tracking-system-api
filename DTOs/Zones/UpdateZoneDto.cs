using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Zones
{
    public class UpdateZoneDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public List<PolygonPointDto> PolygonPoints { get; set; }
            = new List<PolygonPointDto>();
    }
}