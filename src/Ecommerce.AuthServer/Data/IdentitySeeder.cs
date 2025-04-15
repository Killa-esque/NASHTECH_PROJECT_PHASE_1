using Microsoft.AspNetCore.Identity;

namespace Ecommerce.API.Data;
public static class IdentitySeeder
{
  public static async Task SeedAsync(IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var user = await userManager.FindByEmailAsync("user@example.com");
    if (user == null)
    {
      user = new IdentityUser
      {
        UserName = "user@example.com",
        Email = "user@example.com",
        EmailConfirmed = true
      };
      await userManager.CreateAsync(user, "P@ssw0rd!");
    }
  }
}
