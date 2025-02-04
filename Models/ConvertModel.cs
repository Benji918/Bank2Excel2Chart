using System.ComponentModel.DataAnnotations;

namespace Bank2Excel2Chart.Models
{
    public class ConvertModel
    {
        public int Id { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string ImageUrl { get; set; }

    }
}
