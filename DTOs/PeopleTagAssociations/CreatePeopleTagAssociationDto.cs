using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dtos.PeopleTagAssociations
{
    public class CreatePeopleTagAssociationDto
    {
        [Required]
        public int PersonId { get; set; }
        [Required]
        public int TagId { get; set; }
    }
}