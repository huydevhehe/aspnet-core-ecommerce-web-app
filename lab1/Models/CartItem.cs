namespace lab1.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }  // ID của sản phẩm
        public Product Product { get; set; }  // Đối tượng sản phẩm
        public int Quantity { get; set; }  // Số lượng

        public decimal TotalPrice => Product != null ? Product.Price * Quantity : 0;
    }
}
