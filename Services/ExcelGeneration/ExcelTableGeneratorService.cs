using OfficeOpenXml;


namespace Bank2Excel2Chart.Services.ExcelGeneration
{

    public interface IExcelTableGeneratorService
    {
        byte[] GenerateExcelFromTables(List<List<List<string>>> tables);
    }

    public class ExcelTableGeneratorService : IExcelTableGeneratorService
    {
        public byte[] GenerateExcelFromTables(List<List<List<string>>> tables)
        {
            using var package = new ExcelPackage();

            for (int tableIndex = 0; tableIndex < tables.Count; tableIndex++)
            {
                var worksheet = package.Workbook.Worksheets.Add($"Table {tableIndex + 1}");
                var table = tables[tableIndex];

                for (int row = 0; row < table.Count; row++)
                {
                    for (int col = 0; col < table[row].Count; col++)
                    {
                        worksheet.Cells[row + 1, col + 1].Value = table[row][col];
                    }
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            }

            return package.GetAsByteArray();
        }
    }
}
