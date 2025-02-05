using Bank2Excel2Chart.Services.ExcelGeneration;
using Bank2Excel2Chart.Services.PdfProcessing;

namespace Bank2Excel2Chart.Services.Conversion
{
    public interface IEnhancedPdfToExcelConverterService
    {
        byte[] ConvertPdfToExcelWithTables(Stream pdfStream);
    }

    public class Pdf2ExcelConverterService : IEnhancedPdfToExcelConverterService
    {
        private readonly IPdfTableExtractorService _pdfExtractor;
        private readonly IExcelTableGeneratorService _excelGenerator;

        public Pdf2ExcelConverterService(
            IPdfTableExtractorService pdfExtractor,
            IExcelTableGeneratorService excelGenerator)
        {
            _pdfExtractor = pdfExtractor;
            _excelGenerator = excelGenerator;
        }

        public byte[] ConvertPdfToExcelWithTables(Stream pdfStream)
        {
            var extractedTables = _pdfExtractor.ExtractTextFromPdf(pdfStream);
            return _excelGenerator.GenerateExcelFromTables(extractedTables);
        }
    }
}
