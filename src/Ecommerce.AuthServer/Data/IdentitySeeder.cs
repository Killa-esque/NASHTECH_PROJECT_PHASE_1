using Microsoft.AspNetCore.Identity;

namespace Ecommerce.API.Data;
public static class IdentitySeeder
{
  public static async Task SeedAsync(IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var user = await userManager.FindByEmailAsync("pzinh@gmail.com");

    Console.WriteLine($"User: {user == null}");
    if (user == null)
    {
      user = new IdentityUser
      {
        UserName = "pzinh@gmail.com",
        Email = "pzinh@gmail.com",
        EmailConfirmed = true
      };
      var result = await userManager.CreateAsync(user, "@Ee12345");
      if (result.Succeeded)
      {
        Console.WriteLine("User created successfully.");
      }
      else
      {
        Console.WriteLine("Failed to create user:");
        foreach (var error in result.Errors)
        {
          Console.WriteLine($"- {error.Description}");
        }
      }

      // Kiểm tra xem user đã được tạo thành công chưa


    }
  }
}
