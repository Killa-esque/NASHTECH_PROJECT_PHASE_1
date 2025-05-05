using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Data;
using AuthorizationServer.Services;
using AuthorizationServer.Seeders;
using AuthorizationServer.Mapping;
using System.Text.Json;
using AuthorizationServer.Services.Intefaces;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authentication
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
    })
    .AddCookie("SwaggerScheme", options =>
    {
        options.Cookie.Name = ".AspNetCore.Identity.Swagger";
        options.LoginPath = "/Authenticate_Swagger";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AppDbContext>();
    })
    .AddServer(options =>
    {
        options.SetIssuer("https://localhost:5000/");
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetEndSessionEndpointUris("connect/logout")
               .SetTokenEndpointUris("connect/token")
               .SetRevocationEndpointUris("connect/revocation")
               .SetUserInfoEndpointUris("connect/userinfo");

        options.RegisterScopes(Scopes.OpenId, Scopes.Profile, Scopes.Email, Scopes.Roles, "ecommerce_api", Scopes.OfflineAccess);

        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));

        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow();

        options.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        options.DisableAccessTokenEncryption();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableEndSessionEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough();
    });

// Services
builder.Services.AddTransient<AuthorizationService>();
builder.Services.AddTransient<ClientsSeeder>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddRazorPages();

// CORS
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var identitySeeder = new IdentitySeeder(services);
    await identitySeeder.SeedAsync();
    var clientsSeeder = new ClientsSeeder(services);
    await clientsSeeder.SeedAsync();
}

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
