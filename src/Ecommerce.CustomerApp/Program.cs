using Ecommerce.CustomerApp.Services;
using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.CustomerApp.Services.ApiClients;
using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Ecommerce.CustomerApp.ApiClients;

var builder = WebApplication.CreateBuilder(args);

// Configure HttpClient
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "Ecommerce.CustomerApp");
});

// Register ApiClients
builder.Services.AddHttpClient<IUserApiClient, UserApiClient>("ApiClient");
builder.Services.AddHttpClient<ICategoryApiClient, CategoryApiClient>("ApiClient");
builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>("ApiClient");
builder.Services.AddHttpClient<IOrderApiClient, OrderApiClient>("ApiClient");
builder.Services.AddHttpClient<ICartApiClient, CartApiClient>("ApiClient");
builder.Services.AddHttpClient<IRatingApiClient, RatingApiClient>("ApiClient");
builder.Services.AddHttpClient<IUploadApiClient, UploadApiClient>("ApiClient");

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IUploadService, UploadService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpContextAccessor();

// Add Controllers with Views
builder.Services.AddControllersWithViews();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = "https://localhost:5000";
    options.ClientId = "customer_client";
    options.ClientSecret = "CustomerSecret123-4567-89AB-CDEF";
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
        ValidateIssuer = true,
        ValidateAudience = false
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
