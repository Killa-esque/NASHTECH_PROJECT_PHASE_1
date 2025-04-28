namespace Ecommerce.Application.DTOs;

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
}

public class UpdateCustomerDto
{
  public string FullName { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public string Gender { get; set; }
  public string DefaultAddress { get; set; }
  public string AvatarUrl { get; set; }
  public string AllergyNotes { get; set; }
}
