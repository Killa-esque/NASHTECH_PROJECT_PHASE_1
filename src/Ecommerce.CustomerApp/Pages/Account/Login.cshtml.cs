using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

public class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ILogger<LoginModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet(string returnUrl = "/")
    {
        _logger.LogInformation($"LoginModel.OnGet: returnUrl = {returnUrl}");
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = returnUrl
        }, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
