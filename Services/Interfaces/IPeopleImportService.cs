using Microsoft.AspNetCore.Http;
using WebApplication2.DTOs.Imports;

namespace WebApplication2.Services.Interfaces
{
    public interface IPeopleImportService
    {
        Task<PeopleImportSummaryDto> ImportAsync(
            IFormFile file);
    }
}