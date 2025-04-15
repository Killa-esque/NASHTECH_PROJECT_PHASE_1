using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Ecommerce.API.Data // tuỳ namespace của bạn
{
  public class ApplicationDbContext : IdentityDbContext<
      IdentityUser,
      IdentityRole,
      string,
      IdentityUserClaim<string>,
      IdentityUserRole<string>,
      IdentityUserLogin<string>,
      IdentityRoleClaim<string>,
      IdentityUserToken<string>>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // OpenIddict tables
    public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }
  }
}
