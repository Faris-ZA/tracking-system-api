using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Venues
{
    public class UpdateVenueDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;
    }
}