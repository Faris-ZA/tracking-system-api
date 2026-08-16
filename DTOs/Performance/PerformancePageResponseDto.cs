namespace WebApplication2.DTOs.Performance
{
    public class PerformancePageResponseDto<T>
    {
        public string Source { get; set; } = string.Empty;

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int ReturnedRecords { get; set; }

        public long DatabaseQueryTimeMs { get; set; }

        public long CacheQueryTimeMs { get; set; }

        public long TotalExecutionTimeMs { get; set; }

        public List<T> Items { get; set; } = new();
    }
}
