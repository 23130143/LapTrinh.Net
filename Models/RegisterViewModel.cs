using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải từ 10-11 số")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}