namespace WebApplication2.DTOs.Performance
{
    public class DatabasePageResultDto<T>
    {
        public List<T> Items { get; set; } = new();

        public int TotalRecords { get; set; }

        public long DatabaseQueryTimeMs { get; set; }
    }
}