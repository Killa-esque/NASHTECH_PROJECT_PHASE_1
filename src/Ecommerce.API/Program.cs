using System.Text.Json;
using Ecommerce.Application.Common;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Application.Services;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Providers.Storage;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

    });


builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("https://localhost:5000/");
        options.AddAudiences("ecommerce_resource_server");

        options.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        options.UseSystemNetHttp();

        options.UseAspNetCore();
    });


builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.UseAllOfToExtendReferenceSchemas();
    c.CustomSchemaIds(type => type.FullName);

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:5000/connect/authorize"),
                TokenUrl = new Uri("https://localhost:5000/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "ecommerce_api", "resource server scope" },
                    { "offline_access", "Access when offline (refresh token)" }
                }
            },
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "ecommerce_api", "offline_access" }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",  // React Admin
            "https://localhost:5002"  // Customer App (ASP.NET MVC)
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // nếu dùng cookie hoặc auth header
    });
});


builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddHttpClient<ISupabaseStorageService, SupabaseStorageProvider>();

builder.Services.AddTransient(typeof(PagedResultConverter<,>), typeof(PagedResultConverter<,>));
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId("swagger_client");
        c.OAuthClientSecret("901564A5-E7FE-42CB-B10D-61EF6A8F3654"); // nếu không yêu cầu thì bỏ
        c.OAuthScopes("ecommerce_api", "offline_access");
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
