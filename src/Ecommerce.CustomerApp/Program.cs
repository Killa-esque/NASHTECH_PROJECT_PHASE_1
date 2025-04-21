using Ecommerce.CustomerApp.Services;
using Ecommerce.CustomerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configure HttpClient
builder.Services.AddHttpClient("MyHttpClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "Ecommerce.CustomerApp");
});
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddHttpContextAccessor();

// Add Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// ✅ Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = "https://localhost:5000";

    options.ClientId = "customer_client";
    options.ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654";
    options.ResponseType = "code";
    options.UsePkce = false;

    options.SaveTokens = true;
    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.Scope.Add("ecommerce_api");
    options.Scope.Add("offline_access");

    options.GetClaimsFromUserInfoEndpoint = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role",
        ValidateIssuer = true, // bạn có thể set false khi dev local
        ValidateAudience = false // không cần audience nếu không đặt audience trong token
    };

    options.Events = new OpenIdConnectEvents
    {
        OnTokenResponseReceived = ctx =>
        {
            Console.WriteLine("🎉 Token received!");
            Console.WriteLine($"AccessToken: {ctx.TokenEndpointResponse?.AccessToken}");
            Console.WriteLine($"IdToken: {ctx.TokenEndpointResponse?.IdToken}");
            Console.WriteLine($"RefreshToken: {ctx.TokenEndpointResponse?.RefreshToken}");
            return Task.CompletedTask;
        },
        OnRemoteFailure = ctx =>
        {
            ctx.HandleResponse();
            ctx.Response.Redirect("/error?message=" + Uri.EscapeDataString(ctx.Failure?.Message ?? "Unknown error"));
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
