namespace WebApplication2.DTOs.Common
{
    public class ErrorResponseDto
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
