using WebApplication2.Constants;
using WebApplication2.DTOs.Imports;
using WebApplication2.DTOs.People;
using WebApplication2.Exceptions;
using WebApplication2.Services.Interfaces;
using WebApplication2.Services.ImportParsers.Interfaces;


namespace WebApplication2.Services.Implementations
{
    public class PeopleImportService
        : IPeopleImportService
    {
        private const long MaximumFileSizeBytes =
            5 * 1024 * 1024;

        private static readonly string[] SupportedExtensions =
        {
            ".csv",
            ".xlsx"
        };
        private readonly ICsvPeopleImportParser
            _csvParser;

        private readonly IExcelPeopleImportParser
            _excelParser;

        private readonly ILogger<PeopleImportService>
            _logger;

        private readonly IPersonService 
            _personService;

        public PeopleImportService(
            ICsvPeopleImportParser csvParser,
            IExcelPeopleImportParser excelParser,
            IPersonService personService,
            ILogger<PeopleImportService> logger)
        {
            _csvParser = csvParser;
            _excelParser = excelParser;
            _logger = logger;
            _personService = personService;
        }

        public async Task<PeopleImportSummaryDto> ImportAsync(
            IFormFile file)
        {
            ValidateFile(file);

            _logger.LogInformation(
                "People import started. File: {FileName}",
                file.FileName);

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            List<ImportedPersonRowDto> rows;

            if (extension == ".csv")
            {
                rows = _csvParser.Parse(file);
            }
            else
            {
                rows = _excelParser.Parse(file);
            }

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
                        Name = row.Name?.Trim() ?? string.Empty,
                        Phone = row.Phone?.Trim() ?? string.Empty
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

        private static void ValidateFile(
            IFormFile? file)
        {
            if (file == null)
            {
                throw new BadRequestException(
                    ErrorMessages.ImportFileRequired);
            }

            if (file.Length == 0)
            {
                throw new BadRequestException(
                    ErrorMessages.ImportFileEmpty);
            }

            if (file.Length > MaximumFileSizeBytes)
            {
                throw new BadRequestException(
                    ErrorMessages.ImportFileTooLarge);
            }

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            if (!SupportedExtensions.Contains(extension))
            {
                throw new BadRequestException(
                    ErrorMessages.UnsupportedImportFileType);
            }
        }
    }
}