// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("RequireEcommerceApiScope", policy =>
//         policy.RequireAuthenticatedUser()
//               .RequireClaim("scope", "ecommerce_api")
//               .RequireRole("admin"));
//     options.AddPolicy("RequireProfileScope", policy =>
//         policy.RequireAuthenticatedUser()
//               .RequireClaim("scope", "profile"));
// });
using System.Text.Json;
using Ecommerce.Shared.Common;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Application.Services;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Providers.Storage;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Polly;
using Polly.Extensions.Http;
using Ecommerce.Application.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configure database context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:3000", "https://localhost:5002")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure OpenIddict
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("https://localhost:5000/");
        // options.AddAudiences("ecommerce_resource_server");

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        options.UseSystemNetHttp();

        options.UseAspNetCore();
    });

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

// Configure Authorization
builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// HttpClient
builder.Services.AddHttpClient("AuthServerClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5000/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

// Services
builder.Services.AddHttpClient<ISupabaseStorageService, SupabaseStorageProvider>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddTransient(typeof(PagedResultConverter<,>), typeof(PagedResultConverter<,>));
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ResourceServer", Version = "v1" });
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
                    { "ecommerce_api", "Access ecommerce API" },
                    { "profile", "Access User Profile" }
                }
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[] { "ecommerce_api", "profile" }
        }
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId("customer_client");
        c.OAuthScopes("ecommerce_api", "profile");
        c.OAuthClientSecret("CustomerSecret123-4567-89AB-CDEF");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
