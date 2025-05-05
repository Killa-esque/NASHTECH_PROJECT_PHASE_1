using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthorizationServer.Seeders;

public class ClientsSeeder
{
  private readonly IServiceProvider _serviceProvider;

  public ClientsSeeder(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task SeedAsync()
  {
    await AddApiScopeAsync();
    await AddAdminClientAsync();
    await AddCustomerClientAsync();
    await AddSwaggerClientAsync();
  }

  private async Task AddSwaggerClientAsync()
  {
    await using var scope = _serviceProvider.CreateAsyncScope();
    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    var existingClient = await manager.FindByClientIdAsync("swagger_client");
    if (existingClient != null)
    {
      await manager.DeleteAsync(existingClient);
    }

    await manager.CreateAsync(new OpenIddictApplicationDescriptor
    {
      ClientId = "swagger_client",
      ClientSecret = "SwaggerSecret123-4567-89AB-CDEF",
      RedirectUris =
            {
                new Uri("https://localhost:5001/swagger/oauth2-redirect.html")
            },
      Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Endpoints.Revocation,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                $"{Permissions.Prefixes.Scope}ecommerce_api",
                $"{Permissions.Prefixes.Scope}offline_access"
            }
    });
  }

  private async Task AddApiScopeAsync()
  {
    await using var scope = _serviceProvider.CreateAsyncScope();
    var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

    var existingScope = await scopeManager.FindByNameAsync("ecommerce_api");
    if (existingScope != null)
    {
      await scopeManager.DeleteAsync(existingScope);
    }

    await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
    {
      Name = "ecommerce_api",
      DisplayName = "Ecommerce Resource API",
      Resources = { "ecommerce_resource_server" }
    });

    var existingOfflineScope = await scopeManager.FindByNameAsync(Scopes.OfflineAccess);
    if (existingOfflineScope != null)
    {
      await scopeManager.DeleteAsync(existingOfflineScope);
    }

    await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
    {
      Name = Scopes.OfflineAccess,
      DisplayName = "Offline access",
      Resources = { }
    });
  }

  private async Task AddAdminClientAsync()
  {
    await using var scope = _serviceProvider.CreateAsyncScope();
    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    var existingClient = await manager.FindByClientIdAsync("admin_client");
    if (existingClient != null)
    {
      await manager.DeleteAsync(existingClient);
    }

    await manager.CreateAsync(new OpenIddictApplicationDescriptor
    {
      ClientId = "admin_client",
      ClientSecret = null, // SPA không dùng secret
      DisplayName = "Ecommerce Admin Panel (React)",
      ConsentType = ConsentTypes.Explicit,
      RedirectUris =
            {
                new Uri("https://localhost:3000/oauth/callback")
            },
      PostLogoutRedirectUris =
            {
                new Uri("https://localhost:3000/signin")
            },
      Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                $"{Permissions.Prefixes.Scope}ecommerce_api",
                $"{Permissions.Prefixes.Scope}offline_access",
                Permissions.Prefixes.GrantType + "code" // Hỗ trợ PKCE
            }
    });
  }

  private async Task AddCustomerClientAsync()
  {
    await using var scope = _serviceProvider.CreateAsyncScope();
    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    var existingClient = await manager.FindByClientIdAsync("customer_client");
    if (existingClient != null)
    {
      await manager.DeleteAsync(existingClient);
    }

    await manager.CreateAsync(new OpenIddictApplicationDescriptor
    {
      ClientId = "customer_client",
      ClientSecret = "CustomerSecret123-4567-89AB-CDEF",
      DisplayName = "Ecommerce Customer App (ASP.NET MVC)",
      ConsentType = ConsentTypes.Explicit,
      RedirectUris =
            {
                new Uri("https://localhost:5002/signin-oidc")
            },
      PostLogoutRedirectUris =
            {
                new Uri("https://localhost:5002")
            },
      Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                $"{Permissions.Prefixes.Scope}ecommerce_api",
                $"{Permissions.Prefixes.Scope}offline_access"
            }
    });
  }
}
