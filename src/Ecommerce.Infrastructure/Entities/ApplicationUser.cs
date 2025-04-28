using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Infrastructure.Entities;


// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
  public string FullName { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public string Gender { get; set; }
  public string DefaultAddress { get; set; }
  public string AvatarUrl { get; set; }
  public string AllergyNotes { get; set; }
}
