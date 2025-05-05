using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.ViewModels;

public class ProfileViewModel
{
    [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
    public string FullName { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string PhoneNumber { get; set; }

    [RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "Ngày sinh phải có định dạng dd-MM-yyyy")]
    public string DateOfBirth { get; set; }

    [MaxLength(50, ErrorMessage = "Giới tính không được vượt quá 50 ký tự.")]
    public string Gender { get; set; }

    [MaxLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
    public string DefaultAddress { get; set; }

    [MaxLength(500, ErrorMessage = "Ghi chú dị ứng không được vượt quá 500 ký tự.")]
    public string AllergyNotes { get; set; }

    public string AvatarUrl { get; set; }
}

public class ProfileUpdateViewModel
{
    [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
    public string FullName { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string PhoneNumber { get; set; }

    [RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "Ngày sinh phải có định dạng dd-MM-yyyy")]
    public string DateOfBirth { get; set; }

    [MaxLength(50, ErrorMessage = "Giới tính không được vượt quá 50 ký tự.")]
    public string Gender { get; set; }

    [MaxLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
    public string DefaultAddress { get; set; }

    [MaxLength(500, ErrorMessage = "Ghi chú dị ứng không được vượt quá 500 ký tự.")]
    public string AllergyNotes { get; set; }
}
