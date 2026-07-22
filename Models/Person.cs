namespace WebApplication2.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public UpdateStatus UpdateStatus { get; set; } = UpdateStatus.New;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public ICollection<PeopleTagAssociation> TagAssociations { get; set; }
            = new List<PeopleTagAssociation>();
    }
}