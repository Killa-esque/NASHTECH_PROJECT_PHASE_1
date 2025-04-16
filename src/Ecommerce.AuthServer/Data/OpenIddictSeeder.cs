using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Ecommerce.API.Data;
public static class OpenIddictSeeder
{
  public static async Task SeedAsync(IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    if (await manager.FindByClientIdAsync("customer_client") is null)
    {
      await manager.CreateAsync(new OpenIddictApplicationDescriptor
      {
        ClientId = "customer_client",
        ClientSecret = "Mayhabuoi123", // lưu ý: secret sẽ được hash khi lưu DB
        DisplayName = "Customer MVC Client",
        RedirectUris = { new Uri("https://localhost:5002/signin-oidc") },
        PostLogoutRedirectUris = { new Uri("https://localhost:5002/signout-callback-oidc") },
        Permissions =
        {
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            
            // ✅ Cho phép client yêu cầu những scope này:
            "scp:openid",
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Scopes.OfflineAccess,
            "scp:api"
        },
        Requirements =
        {
            // Yêu cầu PKCE
            OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
        }
      });
    }
  }
}
