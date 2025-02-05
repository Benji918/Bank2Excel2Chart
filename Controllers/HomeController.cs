using System.Diagnostics;
using Bank2Excel2Chart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bank2Excel2Chart.Services;
using Bank2Excel2Chart.Services.Conversion;

namespace Bank2Excel2Chart.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEnhancedPdfToExcelConverterService _converter;

        public HomeController(IEnhancedPdfToExcelConverterService converter)
        {
            _converter = converter;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {

            if (file?.Length == 0) return BadRequest("No file uploaded");
            if (Path.GetExtension(file.FileName) != ".pdf") return BadRequest("Only PDF files are allowed");

            try
            {
                using var stream = new MemoryStream();
                file.CopyTo(stream);
                stream.Position = 0;

                var excelBytes = _converter.ConvertPdfToExcelWithTables(stream);

                //Save file to server
                var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_converted.xlsx";
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var fullPath = Path.Combine(savePath, fileName);

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }



                System.IO.File.WriteAllBytes(fullPath, excelBytes);

                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                // Log error
                return StatusCode(500, $"Conversion failed: {ex.Message}");
            }
        }


    }
}
