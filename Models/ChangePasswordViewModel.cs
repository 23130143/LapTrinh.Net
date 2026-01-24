namespace Bookstore.Models;
using System.ComponentModel.DataAnnotations;

    public class ChangePasswordViewModel
    {
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}
