using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer;
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
    var existingUser = await userManager.FindByEmailAsync("user@gmail.com");
    if (existingUser == null)
    {
      // Create a new user
      var user = new ApplicationUser
      {
        UserName = "user",
        Email = "user@gmail.com",
        EmailConfirmed = true
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
