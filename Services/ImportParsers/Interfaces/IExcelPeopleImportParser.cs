using Microsoft.AspNetCore.Http;
using WebApplication2.DTOs.Imports;

namespace WebApplication2.Services.ImportParsers.Interfaces
{
    public interface IExcelPeopleImportParser
    {
        List<ImportedPersonRowDto> Parse(
            IFormFile file);
    }
}