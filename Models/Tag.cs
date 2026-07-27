namespace WebApplication2.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string Label { get; set; } = string.Empty;

        public string Mac { get; set; } = string.Empty;

        public UpdateStatus UpdateStatus { get; set; } = UpdateStatus.New;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public ICollection<PeopleTagAssociation> PeopleAssociations { get; set; }
            = new List<PeopleTagAssociation>();
    }
}