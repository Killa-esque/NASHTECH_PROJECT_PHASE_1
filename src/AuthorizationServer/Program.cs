using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Data;
using AuthorizationServer.Services;
using AuthorizationServer.Seeders;
using AuthorizationServer.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

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
                .SetRevocationEndpointUris("connect/revocation");

        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, Scopes.OfflineAccess, "ecommerce_api");

        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30)); // ví dụ
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.LoginPath = "/Authenticate";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;

});


builder.Services.AddTransient<AuthorizationService>();
builder.Services.AddTransient<ClientsSeeder>();


// Add services to the container.
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
        policy.WithOrigins("https://localhost:5001", "https://localhost:5002", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register AutoMapper
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
