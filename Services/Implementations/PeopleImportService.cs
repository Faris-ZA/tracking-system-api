using WebApplication2.DTOs.Imports;
using WebApplication2.DTOs.People;
using WebApplication2.Exceptions;
using WebApplication2.Services.ImportParsers.Interfaces;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class PeopleImportService
        : IPeopleImportService
    {
        private readonly IPeopleImportFileReader
            _fileReader;

        private readonly ILogger<PeopleImportService>
            _logger;

        private readonly IPersonService
            _personService;

        public PeopleImportService(
            IPeopleImportFileReader fileReader,
            IPersonService personService,
            ILogger<PeopleImportService> logger)
        {
            _fileReader = fileReader;
            _personService = personService;
            _logger = logger;
        }

        public async Task<PeopleImportSummaryDto> ImportAsync(
            IFormFile file)
        {
            _logger.LogInformation(
                "People import started. File: {FileName}",
                file.FileName);

            var rows =
                _fileReader.Read(file);

            var summary = new PeopleImportSummaryDto
            {
                TotalRows = rows.Count
            };

            foreach (var row in rows)
            {
                try
                {
                    var createPersonDto =
                        new CreatePersonDto
                        {
                            Name =
                                row.Name?.Trim()
                                ?? string.Empty,

                            Phone =
                                row.Phone?.Trim()
                                ?? string.Empty
                        };

                    await _personService.CreateAsync(
                        createPersonDto);

                    summary.SuccessfulRows++;
                }
                catch (BadRequestException exception)
                {
                    AddFailure(
                        summary,
                        row.RowNumber,
                        exception.Message);
                }
                catch (ConflictException exception)
                {
                    AddFailure(
                        summary,
                        row.RowNumber,
                        exception.Message);
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Unexpected error while importing row {RowNumber}.",
                        row.RowNumber);

                    AddFailure(
                        summary,
                        row.RowNumber,
                        "The row could not be imported.");
                }
            }

            summary.FailedRows =
                summary.Failures.Count;

            _logger.LogInformation(
                "People import completed. Total: {TotalRows}, Successful: {SuccessfulRows}, Failed: {FailedRows}",
                summary.TotalRows,
                summary.SuccessfulRows,
                summary.FailedRows);

            return summary;
        }

        private static void AddFailure(
            PeopleImportSummaryDto summary,
            int rowNumber,
            string reason)
        {
            summary.Failures.Add(
                new PeopleImportFailureDto
                {
                    RowNumber = rowNumber,
                    Reason = reason
                });
        }
    }
}
