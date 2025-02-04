using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bank2Excel2Chart.Models;

namespace Bank2Excel2Chart.Data
{
    public class ApplicationDBcontext : IdentityDbContext
    {
        public DbSet<ConvertModel> Bank2Excel2Charts { get; set; }
        public ApplicationDBcontext(DbContextOptions<ApplicationDBcontext> options) : base(options)
        {

        }
    }
}
