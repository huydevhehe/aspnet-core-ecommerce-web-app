using System.ComponentModel.DataAnnotations;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    [RegularExpression(@"^[a-zA-ZÀ-Ỹà-ỹ\s]+$", ErrorMessage = "Họ và tên không được chứa ký tự đặc biệt hoặc số.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Địa chỉ không được để trống.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số.")]
    public string PhoneNumber { get; set; }
}
