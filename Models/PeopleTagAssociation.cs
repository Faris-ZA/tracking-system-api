using WebApplication2.Models;

namespace WebApplication2.Models
{
    public class PeopleTagAssociation
{
    public int PeopleId { get; set; }

    public int TagId { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;

    public Person Person { get; set; } = null!;

    public Tag Tag { get; set; } = null!;
}
}