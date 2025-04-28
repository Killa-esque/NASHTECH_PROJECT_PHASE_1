using Ecommerce.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Seeders;
public class IdentitySeeder
{
  private readonly IServiceProvider _serviceProvider;

  public IdentitySeeder(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task SeedAsync()
  {
    await AddUser1Async();
  }

  private async Task AddUser1Async()
  {
    var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Check if the user already exists
    var existingUser = await userManager.FindByEmailAsync("pzinh@gmail.com");
    if (existingUser == null)
    {
      // Create a new user
      var user = new ApplicationUser
      {
        UserName = "Killa",
        Email = "pzinh@gmail.com",
        EmailConfirmed = true,
        FullName = "Vo Phu Vinh",
        DateOfBirth = new DateTime(2003, 10, 15),
        Gender = "Male",
        DefaultAddress = "180 Nguyen Van Cu, District 5, HCM City",
        AvatarUrl = "",
        AllergyNotes = "None"
      };

      // Set the user's password
      var result = await userManager.CreateAsync(user, "Password123@");

      if (!result.Succeeded)
      {
        throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }
    }
  }
}
