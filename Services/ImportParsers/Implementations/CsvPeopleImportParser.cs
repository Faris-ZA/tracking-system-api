using Microsoft.VisualBasic.FileIO;
using WebApplication2.Constants;
using WebApplication2.DTOs.Imports;
using WebApplication2.Exceptions;
using WebApplication2.Services.ImportParsers.Interfaces;

namespace WebApplication2.Services.ImportParsers.Implementations
{
    public class CsvPeopleImportParser
        : ICsvPeopleImportParser
    {
        public List<ImportedPersonRowDto> Parse(
            IFormFile file)
        {
            var rows =
                new List<ImportedPersonRowDto>();

            using var stream =
                file.OpenReadStream();

            using var parser =
                new TextFieldParser(stream);

            parser.TextFieldType =
                FieldType.Delimited;

            parser.SetDelimiters(",");

            parser.HasFieldsEnclosedInQuotes =
                true;

            if (parser.EndOfData)
            {
                throw new BadRequestException(
                    ErrorMessages.ImportFileEmpty);
            }

            var headers =
                parser.ReadFields();

            if (headers == null)
            {
                throw new BadRequestException(
                    ErrorMessages.InvalidImportHeaders);
            }

            var nameIndex =
                FindHeaderIndex(
                    headers,
                    "Name");

            var phoneIndex =
                FindHeaderIndex(
                    headers,
                    "Phone");

            if (nameIndex == -1 ||
                phoneIndex == -1)
            {
                throw new BadRequestException(
                    ErrorMessages.InvalidImportHeaders);
            }

            var rowNumber = 1;

            while (!parser.EndOfData)
            {
                rowNumber++;

                try
                {
                    var fields =
                        parser.ReadFields();

                    if (fields == null ||
                        fields.All(
                            string.IsNullOrWhiteSpace))
                    {
                        continue;
                    }

                    var name =
                        nameIndex < fields.Length
                            ? fields[nameIndex]
                            : string.Empty;

                    var phone =
                        phoneIndex < fields.Length
                            ? fields[phoneIndex]
                            : string.Empty;

                    rows.Add(
                        new ImportedPersonRowDto
                        {
                            RowNumber = rowNumber,
                            Name = name,
                            Phone = phone
                        });
                }
                catch (MalformedLineException)
                {
                    rows.Add(
                        new ImportedPersonRowDto
                        {
                            RowNumber = rowNumber,
                            Name = string.Empty,
                            Phone = string.Empty
                        });
                }
            }

            return rows;
        }

        private static int FindHeaderIndex(
            string[] headers,
            string expectedHeader)
        {
            return Array.FindIndex(
                headers,
                header =>
                    string.Equals(
                        header.Trim(),
                        expectedHeader,
                        StringComparison.OrdinalIgnoreCase));
        }
    }
}