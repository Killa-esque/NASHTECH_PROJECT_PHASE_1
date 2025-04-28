// Ecommerce.Shared/DTOs/CustomerDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs;

public class CustomerDto
{
  public string Id { get; set; }
  public string UserName { get; set; }
  public string Email { get; set; }
  public string FullName { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public string Gender { get; set; }
  public string DefaultAddress { get; set; }
  public string AvatarUrl { get; set; }
  public string AllergyNotes { get; set; }

  public CustomerDto() { }
}

public class UpdateCustomerDto
{
  [Required]
  [MaxLength(100)]
  public string FullName { get; set; }

  [RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "DateOfBirth must be in format dd-MM-yyyy")]
  public string? DateOfBirth { get; set; }

  [MaxLength(50)]
  public string? Gender { get; set; }

  [MaxLength(200)]
  public string? DefaultAddress { get; set; }

  [Url]
  public string? AvatarUrl { get; set; }

  [MaxLength(500)]
  public string? AllergyNotes { get; set; }

  public UpdateCustomerDto() { }
}
