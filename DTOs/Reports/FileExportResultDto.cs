namespace WebApplication2.DTOs.Reports
{
    public class FileExportResultDto
    {
        public byte[] Content { get; set; } =
            Array.Empty<byte>();

        public string FileName { get; set; } =
            string.Empty;

        public string ContentType { get; set; } =
            string.Empty;
    }
}