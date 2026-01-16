namespace lab1.Models
{
    public class CouponModel
    {
        public long Id { get; set; }  // ID của coupon
        public string Name { get; set; } = string.Empty;  // Tên coupon
        public string Description { get; set; } = string.Empty;  // Mô tả coupon
        public int Quantity { get; set; }  // Số lượng coupon còn lại
        public DateTime DateExpired { get; set; }  // Ngày hết hạn
    }
}
