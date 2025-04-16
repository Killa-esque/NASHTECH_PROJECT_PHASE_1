using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce.API.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/account/login";
    options.ReturnUrlParameter = "returnUrl";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie.Name = "AuthServer";

    // Thêm các dòng sau:
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // nếu dev dùng HTTPS
});

builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        opt.UseEntityFrameworkCore()
           .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(opt =>
    {
        opt.SetAuthorizationEndpointUris("connect/authorize")
           .SetTokenEndpointUris("connect/token")
           .SetRevocationEndpointUris("connect/revoke");

        opt.AllowAuthorizationCodeFlow()
           .RequireProofKeyForCodeExchange()
           .AllowRefreshTokenFlow();

        opt.AddDevelopmentSigningCertificate()
           .AddDevelopmentEncryptionCertificate();

        opt.DisableAccessTokenEncryption();

        opt.UseAspNetCore()
           .EnableAuthorizationEndpointPassthrough() // KHÔNG bật dòng này
           .EnableTokenEndpointPassthrough()
           .DisableTransportSecurityRequirement();

        opt.RegisterScopes("openid", "profile", "offline_access", "api");
    });

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole()
           .AddFilter("OpenIddict", LogLevel.Debug);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedAsync(services);
    await OpenIddictSeeder.SeedAsync(services);
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
