using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;

namespace Bank2Excel2Chart.Services.PdfProcessing
{
    public interface IPdfTableExtractorService
    {
        List<List<List<string>>> ExtractTextFromPdf(Stream pdfStream);
    }

    public class PdfTableExtractorService : IPdfTableExtractorService
    {
        public List<List<List<string>>> ExtractTextFromPdf(Stream pdfStream)
        {
            var tables = new List<List<List<string>>>();
            var reader = new PdfReader(pdfStream);

            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                var strategy = new TableDetectionStrategy();
                var currentPageText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);

                if (strategy.DetectedTables.Count > 0)
                {
                    tables.AddRange(strategy.DetectedTables);
                }
            }

            reader.Close();
            return tables;
        }
    }

    // Custom strategy for table detection
    public class TableDetectionStrategy : LocationTextExtractionStrategy
    {
        public List<List<List<string>>> DetectedTables { get; } = new List<List<List<string>>>();
        private readonly List<TextChunk> _chunks = new List<TextChunk>();

        public override void RenderText(TextRenderInfo renderInfo)
        {
            var chunk = new TextChunk
            {
                Text = renderInfo.GetText(),
                Bounds = new Rectangle(
                    renderInfo.GetDescentLine().GetStartPoint()[0],
                    renderInfo.GetDescentLine().GetStartPoint()[1],
                    renderInfo.GetAscentLine().GetEndPoint()[0],
                    renderInfo.GetAscentLine().GetEndPoint()[1]
                )
            };
            _chunks.Add(chunk);
        }

        public override string GetResultantText()
        {
            // Group text chunks into rows and columns based on coordinates
            var table = new List<List<string>>();
            var currentRow = new List<string>();
            var yPositions = new SortedSet<float>();
            var xPositions = new SortedDictionary<float, List<TextChunk>>();

            // First pass to get Y coordinates (rows)
            foreach (var chunk in _chunks.OrderBy(c => c.Bounds.Top).Reverse())
            {
                yPositions.Add(chunk.Bounds.Top);
            }

            // Second pass to group by X coordinates (columns)
            foreach (var y in yPositions)
            {
                var rowChunks = _chunks.Where(c => Math.Abs(c.Bounds.Top - y) < 1).OrderBy(c => c.Bounds.Left);
                var row = new List<string>();
                string currentCell = "";

                foreach (var chunk in rowChunks)
                {
                    // Simple column detection (could be improved)
                    if (!string.IsNullOrWhiteSpace(currentCell) &&
                        chunk.Bounds.Left - rowChunks.Last().Bounds.Right > 5)
                    {
                        row.Add(currentCell.Trim());
                        currentCell = "";
                    }
                    currentCell += chunk.Text + " ";
                }

                if (!string.IsNullOrWhiteSpace(currentCell))
                {
                    row.Add(currentCell.Trim());
                }

                if (row.Count > 0)
                {
                    table.Add(row);
                }
            }

            DetectedTables.Add(table);
            return string.Empty;
        }

        private class TextChunk
        {
            public string Text { get; set; }
            public Rectangle Bounds { get; set; }
        }
    }
}
