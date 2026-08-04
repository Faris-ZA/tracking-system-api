using WebApplication2.Constants;
using WebApplication2.DTOs.Imports;
using WebApplication2.Exceptions;
using WebApplication2.Services.ImportParsers.Interfaces;

namespace WebApplication2.Infrastructure.Import
{
    public class PeopleImportFileReader
        : IPeopleImportFileReader
    {
        private const long MaximumFileSizeBytes =
            5 * 1024 * 1024;

        private const string CsvExtension = ".csv";

        private static readonly string[] SupportedExtensions =
        {
            CsvExtension,
            ".xlsx"
        };

        private readonly ICsvPeopleImportParser
            _csvParser;

        private readonly IExcelPeopleImportParser
            _excelParser;

        public PeopleImportFileReader(
            ICsvPeopleImportParser csvParser,
            IExcelPeopleImportParser excelParser)
        {
            _csvParser = csvParser;
            _excelParser = excelParser;
        }

        public List<ImportedPersonRowDto> Read(
            IFormFile file)
        {
            ValidateFile(file);

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            if (extension == CsvExtension)
            {
                return _csvParser.Parse(file);
            }

            return _excelParser.Parse(file);
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