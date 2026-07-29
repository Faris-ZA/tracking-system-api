using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Zones
{
    public class CreateZoneDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int FloorId { get; set; }

        [Required]
        public List<PolygonPointDto> PolygonPoints { get; set; }
            = new List<PolygonPointDto>();
    }
}