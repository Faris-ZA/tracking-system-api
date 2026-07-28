using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Positions
{
    public class CreatePositionDto
    {
        [Required]
        public int PersonId { get; set; }

        [Required]
        public int VenueId { get; set; }

        [Required]
        public int FloorId { get; set; }

        [Required]
        public int ZoneId { get; set; }

        [Required]
        public int X { get; set; }

        [Required]
        public int Y { get; set; }
    }
}