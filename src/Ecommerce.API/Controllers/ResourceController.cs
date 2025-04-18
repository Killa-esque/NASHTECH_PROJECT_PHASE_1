using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OAuth.OpenIddict.ResourceServer.Controllers;

[ApiController]
[Route("resources")]
public class ResourceController : Controller
{
  [Authorize]
  [HttpGet]
  public Task<IActionResult> GetSecretResources()
  {
    var user = HttpContext.User?.Identity?.Name;
    return Task.FromResult<IActionResult>(Ok($"user: {user}"));
  }
}
