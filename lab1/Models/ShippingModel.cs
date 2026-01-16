using System.ComponentModel.DataAnnotations.Schema;

namespace lab1.Models
{
    public class ShippingModel
    {
        public int Id { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}