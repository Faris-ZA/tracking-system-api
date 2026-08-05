using ClosedXML.Excel;
using WebApplication2.Constants;
using WebApplication2.DTOs.Imports;
using WebApplication2.Exceptions;
using WebApplication2.Services.ImportParsers.Interfaces;

namespace WebApplication2.Infrastructure.Import
{
    public class ExcelPeopleImportParser
        : IExcelPeopleImportParser
    {
        public List<ImportedPersonRowDto> Parse(
            IFormFile file)
        {
            var rows =
                new List<ImportedPersonRowDto>();

            using var stream =
                file.OpenReadStream();

            using var workbook =
                new XLWorkbook(stream);

            var worksheet =
                workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                throw new BadRequestException(
                    ErrorMessages.ImportFileEmpty);
            }

            var usedRange =
                worksheet.RangeUsed();

            if (usedRange == null)
            {
                throw new BadRequestException(
                    ErrorMessages.ImportFileEmpty);
            }

            var headerRow =
                usedRange.FirstRow();

            var nameColumnNumber =
                FindHeaderColumn(
                    headerRow,
                    "Name");

            var phoneColumnNumber =
                FindHeaderColumn(
                    headerRow,
                    "Phone");

            if (nameColumnNumber == -1 ||
                phoneColumnNumber == -1)
            {
                throw new BadRequestException(
                    ErrorMessages.InvalidImportHeaders);
            }

            var firstDataRowNumber =
                headerRow.RowNumber() + 1;

            var lastUsedRowNumber =
                usedRange.LastRow().RowNumber();

            for (
                var rowNumber = firstDataRowNumber;
                rowNumber <= lastUsedRowNumber;
                rowNumber++)
            {
                var row =
                    worksheet.Row(rowNumber);

                var name =
                    row.Cell(nameColumnNumber)
                        .GetString();

                var phone =
                    row.Cell(phoneColumnNumber)
                        .GetString();

                if (string.IsNullOrWhiteSpace(name) &&
                    string.IsNullOrWhiteSpace(phone))
                {
                    continue;
                }

                rows.Add(
                    new ImportedPersonRowDto
                    {
                        RowNumber = rowNumber,
                        Name = name,
                        Phone = phone
                    });
            }

            return rows;
        }

        private static int FindHeaderColumn(
            IXLRangeRow headerRow,
            string expectedHeader)
        {
            foreach (var cell in headerRow.CellsUsed())
            {
                var headerValue =
                    cell.GetString().Trim();

                if (string.Equals(
                        headerValue,
                        expectedHeader,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return cell.Address.ColumnNumber;
                }
            }

            return -1;
        }
    }
}
