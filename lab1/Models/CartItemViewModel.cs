using lab1.Models;  // Thêm using

namespace lab1.Models
{
    public class CartItemViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>(); // Đổi từ CartItem -> CartItemModel
        public decimal GrandTotal { get; set; }
        public decimal ShippingPrice { get; set; }
        public string CouponCode { get; set; } = string.Empty;
    }
}
