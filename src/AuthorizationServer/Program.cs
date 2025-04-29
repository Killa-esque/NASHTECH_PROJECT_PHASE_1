using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Data;
using AuthorizationServer.Services;
using AuthorizationServer.Seeders;
using AuthorizationServer.Mapping;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json;
using OpenIddict.Server;
using OpenIddict.Abstractions;
using System.Security.Claims;
using Microsoft.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ⚙️ Cấu hình nhiều cookie scheme: AdminScheme & CustomerScheme
builder.Services.AddAuthentication()
    .AddCookie("AdminScheme", options =>
    {
        options.Cookie.Name = ".AspNetCore.Identity.Admin";
        options.LoginPath = "/Authenticate_Admin";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    })
    .AddCookie("CustomerScheme", options =>
    {
        options.Cookie.Name = ".AspNetCore.Identity.Customer";
        options.LoginPath = "/Authenticate_Customer";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.Cookie.Name = ".AspNetCore.Identity";
//     options.LoginPath = "/Authenticate";
//     options.ExpireTimeSpan = TimeSpan.FromDays(7);
//     options.SlidingExpiration = true;
// });


builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetEndSessionEndpointUris("connect/logout")
               .SetTokenEndpointUris("connect/token")
               .SetRevocationEndpointUris("connect/revocation")
               .SetUserInfoEndpointUris("connect/userinfo");

        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, Scopes.OfflineAccess, "ecommerce_api");

        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));

        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow();

        options.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableEndSessionEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough();

    });

builder.Services.AddTransient<AuthorizationService>();
builder.Services.AddTransient<ClientsSeeder>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:5001", "https://localhost:5002", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var identitySeeder = new IdentitySeeder(services);
    await identitySeeder.SeedAsync();

    var clientsSeeder = new ClientsSeeder(services);
    await clientsSeeder.SeedAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
