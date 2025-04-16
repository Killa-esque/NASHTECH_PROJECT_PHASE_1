using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = "https://localhost:5001"; // AuthServer URL
    options.ClientId = "customer_client"; // Client ID
    options.ClientSecret = "Mayhabuoi123"; // Client secret
    options.ResponseType = "code";
    options.UsePkce = true;
    options.SaveTokens = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("api");
    options.Scope.Add("offline_access");

    options.GetClaimsFromUserInfoEndpoint = false;
    // options.TokenValidationParameters.NameClaimType = "sub";
    // options.TokenValidationParameters.RoleClaimType = "role";
    // options.TokenValidationParameters.ValidateIssuer = false; // Chỉ dùng cho dev môi trường
    // options.TokenValidationParameters.ValidateAudience = false; // Chỉ dùng cho dev môi trường

});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
