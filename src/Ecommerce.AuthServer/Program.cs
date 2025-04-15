using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Ecommerce.API.Data;
using Ecommerce.API.Data;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Tell OpenIddict to use EF Core with this context
    options.UseOpenIddict();
});


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        opt.UseEntityFrameworkCore()
           .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(opt =>
    {
        opt.SetTokenEndpointUris("connect/token");
        opt.SetAuthorizationEndpointUris("connect/authorize");

        opt.AllowAuthorizationCodeFlow()
           .RequireProofKeyForCodeExchange();

        opt.RegisterScopes("openid", "profile", "email", "roles");

        opt.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();


        opt.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .DisableTransportSecurityRequirement();
    })
    .AddValidation(opt =>
    {
        opt.UseLocalServer();
        opt.UseAspNetCore();
    });

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await IdentitySeeder.SeedAsync(services);
    await OpenIddictSeeder.SeedAsync(services);
}

// app.UseHttpsRedirection();
// app.UseStaticFiles();

// app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// app.UseCustomMiddlewares();

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
//     endpoints.MapDefaultControllerRoute();
// });

// app.MapDefaultControllerRoute();
app.MapControllers();

// Create a endpoint to test
app.MapGet("/test", () => "Hello World!");

app.Run();




