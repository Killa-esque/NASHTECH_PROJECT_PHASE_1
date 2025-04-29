using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthorizationServer.Services;

public class AuthorizationService
{
  public IDictionary<string, StringValues> ParseOAuthParameters(HttpContext httpContext, List<string>? excluding = null)
  {
    excluding ??= new List<string>();

    var parameters = httpContext.Request.HasFormContentType
        ? httpContext.Request.Form
            .Where(v => !excluding.Contains(v.Key))
            .ToDictionary(v => v.Key, v => v.Value)
        : httpContext.Request.Query
            .Where(v => !excluding.Contains(v.Key))
            .ToDictionary(v => v.Key, v => v.Value);

    return parameters;
  }

  public string BuildRedirectUrl(HttpRequest request, IDictionary<string, StringValues> oAuthParameters)
  {
    var url = request.PathBase + request.Path + QueryString.Create(oAuthParameters);
    return url;
  }

  public bool IsAuthenticated(AuthenticateResult authenticateResult, OpenIddictRequest request)
  {
    if (!authenticateResult.Succeeded)
    {
      return false;
    }

    if (request.MaxAge.HasValue && authenticateResult.Properties != null)
    {
      var maxAgeSeconds = TimeSpan.FromSeconds(request.MaxAge.Value);

      var expired = !authenticateResult.Properties.IssuedUtc.HasValue ||
                    DateTimeOffset.UtcNow - authenticateResult.Properties.IssuedUtc > maxAgeSeconds;
      if (expired)
      {
        return false;
      }
    }

    return true;
  }

  public static IEnumerable<string> GetDestinations(ClaimsIdentity identity, Claim claim)
  {
    // Validate inputs to prevent runtime errors
    if (identity == null || claim == null)
    {
      throw new ArgumentNullException(identity == null ? nameof(identity) : nameof(claim));
    }

    switch (claim.Type)
    {
      case Claims.Subject: // sub
      case Claims.Name:    // name
      case Claims.Email:   // email
        return new[] { Destinations.AccessToken, Destinations.IdentityToken };

      case Claims.Role:
        return new[] { Destinations.AccessToken, Destinations.IdentityToken };

      default:
        return Array.Empty<string>(); 
    }
  }
}
