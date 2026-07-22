namespace WebApplication2.Models
{
    public class Venue
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public UpdateStatus UpdateStatus { get; set; } = UpdateStatus.New;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public ICollection<Floor> Floors { get; set; } = new List<Floor>();
    }
}