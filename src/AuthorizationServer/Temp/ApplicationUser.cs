using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Temp;


public class ApplicationUser : IdentityUser
{
  public string FullName { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public string Gender { get; set; }
  public string DefaultAddress { get; set; }
  public string AvatarUrl { get; set; }
  public string AllergyNotes { get; set; }
}
