using WebApplication2.DTOs.Reports;

namespace WebApplication2.Services.Interfaces
{
    public interface IPeopleLocationExportService
    {
        Task<FileExportResultDto>
            ExportAllPeopleAsync();
    }
}