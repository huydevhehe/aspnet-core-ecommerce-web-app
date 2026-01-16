using System.ComponentModel.DataAnnotations.Schema;

namespace lab1.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; } // Thêm quan hệ với Order

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!; // Đảm bảo Product luôn có giá trị

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
