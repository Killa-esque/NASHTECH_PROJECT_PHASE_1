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
    // Tạo vai trò trước
    await AddRolesAsync();
    // Tạo người dùng sau
    await AddUserWithRoleAsync("pzinh@gmail.com", "Killa", "Password123@", "Vo Phu Vinh", "customer");
    await AddUserWithRoleAsync("admin@example.com", "AdminUser", "Admin123@", "Admin Name", "admin");
  }

  private async Task AddRolesAsync()
  {
    var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Tạo vai trò "user" nếu chưa tồn tại
    if (!await roleManager.RoleExistsAsync("customer"))
    {
      var result = await roleManager.CreateAsync(new IdentityRole("customer"));
      if (!result.Succeeded)
      {
        throw new Exception($"Failed to create role 'user': {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }
      Console.WriteLine("[SEED] ✅ Created role: user");
    }

    // Tạo vai trò "admin" nếu chưa tồn tại
    if (!await roleManager.RoleExistsAsync("admin"))
    {
      var result = await roleManager.CreateAsync(new IdentityRole("admin"));
      if (!result.Succeeded)
      {
        throw new Exception($"Failed to create role 'admin': {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }
      Console.WriteLine("[SEED] ✅ Created role: admin");
    }
  }

  private async Task AddUserWithRoleAsync(string email, string userName, string password, string fullName, string role)
  {
    var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Kiểm tra nếu người dùng đã tồn tại
    var existingUser = await userManager.FindByEmailAsync(email);
    if (existingUser == null)
    {
      // Tạo người dùng mới
      var user = new ApplicationUser
      {
        UserName = userName,
        Email = email,
        EmailConfirmed = true,
        FullName = fullName,
        DateOfBirth = new DateTime(2003, 10, 15),
        Gender = "Male",
        DefaultAddress = "180 Nguyen Van Cu, District 5, HCM City",
        AvatarUrl = "",
        AllergyNotes = "None"
      };

      // Tạo người dùng với mật khẩu
      var result = await userManager.CreateAsync(user, password);
      if (!result.Succeeded)
      {
        throw new Exception($"Failed to create user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }
      Console.WriteLine($"[SEED] ✅ Created user: {email}");

      // Gán vai trò
      var roleResult = await userManager.AddToRoleAsync(user, role);
      if (!roleResult.Succeeded)
      {
        throw new Exception($"Failed to assign role '{role}' to user {email}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
      }
      Console.WriteLine($"[SEED] ✅ Assigned role '{role}' to user: {email}");
    }
    else
    {
      Console.WriteLine($"[SEED] ℹ️ User {email} already exists, skipping creation.");
    }
  }
}
