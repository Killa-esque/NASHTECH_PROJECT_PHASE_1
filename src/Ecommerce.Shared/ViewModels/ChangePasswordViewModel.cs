using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.ViewModels;

public class ChangePasswordViewModel
{
  [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc.")]
  [MinLength(6, ErrorMessage = "Mật khẩu hiện tại phải dài ít nhất 6 ký tự.")]
  public string CurrentPassword { get; set; }

  [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
  [MinLength(6, ErrorMessage = "Mật khẩu mới phải dài ít nhất 6 ký tự.")]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
      ErrorMessage = "Mật khẩu mới phải chứa ít nhất một chữ hoa, một chữ thường, một số và một ký tự đặc biệt.")]
  public string NewPassword { get; set; }

  [Required(ErrorMessage = "Xác nhận mật khẩu mới là bắt buộc.")]
  [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu mới không khớp.")]
  public string ConfirmNewPassword { get; set; }
}
