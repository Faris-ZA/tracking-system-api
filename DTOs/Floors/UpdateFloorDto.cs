using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTOs.Floors
{
    public class UpdateFloorDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Level { get; set; }
    }
}