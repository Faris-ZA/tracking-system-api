using WebApplication2.DTOs.Imports;

namespace WebApplication2.Services.ImportParsers.Interfaces
{
    public interface IPeopleImportFileReader
    {
        List<ImportedPersonRowDto> Read(
            IFormFile file);
    }
}