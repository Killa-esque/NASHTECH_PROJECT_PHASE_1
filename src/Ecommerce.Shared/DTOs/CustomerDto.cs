using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs;

public class CustomerDto
{
  public string Id { get; set; }
  public string UserName { get; set; }
  public string Email { get; set; }
  public string FullName { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public string PhoneNumber { get; set; }
  public string Gender { get; set; }
  public string DefaultAddress { get; set; }
  public string AvatarUrl { get; set; }
  public string AllergyNotes { get; set; }
}

public class CreateCustomerDto
{
  [Required(ErrorMessage = "Email is required.")]
  [EmailAddress(ErrorMessage = "Invalid email format.")]
  public string Email { get; set; }

  [Required(ErrorMessage = "Password is required.")]
  [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
      ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
  public string Password { get; set; }

  [Required(ErrorMessage = "Full name is required.")]
  [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
  public string FullName { get; set; }

  [RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "DateOfBirth must be in format dd-MM-yyyy")]
  public string DateOfBirth { get; set; }

  [MaxLength(50, ErrorMessage = "Gender cannot exceed 50 characters.")]
  public string Gender { get; set; }

  [MaxLength(200, ErrorMessage = "Default address cannot exceed 200 characters.")]
  public string DefaultAddress { get; set; }

  [MaxLength(500, ErrorMessage = "Allergy notes cannot exceed 500 characters.")]
  public string AllergyNotes { get; set; }

  [Phone(ErrorMessage = "Invalid phone number format.")]
  public string PhoneNumber { get; set; }
}

public class UpdateCustomerDto
{
  [Required(ErrorMessage = "Full name is required.")]
  [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
  public string FullName { get; set; }

  [RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "DateOfBirth must be in format dd-MM-yyyy")]
  public string DateOfBirth { get; set; }

  [MaxLength(50, ErrorMessage = "Gender cannot exceed 50 characters.")]
  public string Gender { get; set; }

  [MaxLength(200, ErrorMessage = "Default address cannot exceed 200 characters.")]
  public string DefaultAddress { get; set; }

  [MaxLength(500, ErrorMessage = "Allergy notes cannot exceed 500 characters.")]
  public string AllergyNotes { get; set; }

  [Phone(ErrorMessage = "Invalid phone number format.")]
  public string PhoneNumber { get; set; }
}

public class ChangePasswordDto
{
  [Required(ErrorMessage = "Current password is required.")]
  public string CurrentPassword { get; set; }

  [Required(ErrorMessage = "New password is required.")]
  public string NewPassword { get; set; }
}

public class UpdateAvatarDto
{
  [Required(ErrorMessage = "Avatar URL is required.")]
  public string AvatarUrl { get; set; }
}
