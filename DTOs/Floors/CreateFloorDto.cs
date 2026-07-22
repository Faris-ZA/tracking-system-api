using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Floors
{
    public class CreateFloorDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int VenueId { get; set; }

        [Required]
        public int Level { get; set; }
    }
}