using Bank2Excel2Chart.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Bank2Excel2Chart.Services;
using Bank2Excel2Chart.Services.PdfProcessing;
using Bank2Excel2Chart.Services.ExcelGeneration;
using Bank2Excel2Chart.Services.Conversion;
using System.Text;

namespace Bank2Excel2Chart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Add DBContext
            builder.Services.AddDbContext<ApplicationDBcontext>(
                options =>
                {
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DB"))
                     .LogTo(Console.WriteLine, LogLevel.Information);
                });

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDBcontext>();

            //Add Interface services
            builder.Services.AddScoped<IPdfTableExtractorService, PdfTableExtractorService>();
            builder.Services.AddScoped<IExcelTableGeneratorService, ExcelTableGeneratorService>();
            builder.Services.AddScoped<IEnhancedPdfToExcelConverterService, Pdf2ExcelConverterService>();

            // Required for iTextSharp
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);




            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
