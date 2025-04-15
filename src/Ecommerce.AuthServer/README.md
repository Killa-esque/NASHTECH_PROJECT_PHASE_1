# ✅ Ecommerce.AuthServer – README Checklist TỪ A-Z (OpenIddict + Clean Architecture)

Đây là tài liệu hướng dẫn từng bước để triển khai **AuthServer riêng** cho hệ thống ecommerce, sử dụng **ASP.NET Core + OpenIddict + ASP.NET Identity**. Hướng dẫn này đảm bảo bạn có thể login từ các client như Razor Pages (CustomerApp), React SPA (AdminApp) và bảo mật toàn bộ hệ thống đúng chuẩn **OAuth2 + OpenID Connect**.

---

## 🧱 TỔNG THỂ KIẾN TRÚC

```text
[ CustomerApp / AdminApp / MobileApp ]
                │
        🔁 Redirect to login
                │
         ▼ AuthServer (OpenIddict)
          - /connect/authorize
          - /connect/token
          - /connect/userinfo
          - /api/me (optional)
                │
         access_token, id_token, refresh_token
                │
         ▼ Ecommerce.API (Bảo vệ bằng [Authorize])
```

---

## 📦 DANH SÁCH PROJECT TRONG GIẢI PHÁP

| Project | Vai trò |
|--------|---------|
| **Ecommerce.AuthServer** | Xác thực, cấp token, quản lý user |
| **Ecommerce.API**         | Resource API, cung cấp dữ liệu có bảo vệ |
| **CustomerApp (Razor)**   | Giao diện frontend, login bằng OIDC |
| **AdminApp (React)**      | SPA, dùng PKCE login bằng OIDC |

---

## ✅ CHECKLIST A-Z – DỰNG AUTH SERVER

### 🟩 STEP 1: Khởi tạo project AuthServer

```bash
dotnet new webapp -n Ecommerce.AuthServer
```

### 🟩 STEP 2: Cài package cần thiết

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
.dotnet add package Microsoft.EntityFrameworkCore.SqlServer
.dotnet add package OpenIddict.AspNetCore
.dotnet add package OpenIddict.EntityFrameworkCore
```

### 🟩 STEP 3: Cấu hình DbContext, Identity, OpenIddict

- Tạo `ApplicationUser : IdentityUser`
- Tạo `ApplicationDbContext : IdentityDbContext<ApplicationUser>`
- Cấu hình `Program.cs` để:
  - `AddIdentity()` + cấu hình cookie path
  - `AddOpenIddict().AddServer(...)`
  - `AddControllersWithViews()`

### 🟩 STEP 4: Thêm Razor Views login, register, logout

- Tạo `AccountController`
- Tạo Views:
  - `Views/Account/Login.cshtml`
  - `Views/Account/Register.cshtml`
  - `Views/Account/Logout.cshtml`

### 🟩 STEP 5: Cấu hình Endpoint OpenIddict

```csharp
options.SetAuthorizationEndpointUris("/connect/authorize")
       .SetTokenEndpointUris("/connect/token")
       .SetUserinfoEndpointUris("/connect/userinfo")
       .SetLogoutEndpointUris("/connect/logout");
```

- Kích hoạt flow:
```csharp
options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
options.AllowRefreshTokenFlow();
```

- Passthrough cho UI:
```csharp
options.UseAspNetCore()
       .EnableAuthorizationEndpointPassthrough()
       .EnableTokenEndpointPassthrough()
       .EnableUserinfoEndpointPassthrough();
```

---

## ✅ CHECKLIST TẠO CLIENT APP (`client_id`)

- Viết class `ClientSeeder.cs`
- Seed client trong `Program.cs`

```csharp
await manager.CreateAsync(new OpenIddictApplicationDescriptor
{
    ClientId = "customer_app",
    RedirectUris = { new Uri("https://localhost:5002/signin-oidc") },
    PostLogoutRedirectUris = { new Uri("https://localhost:5002/signout-callback-oidc") },
    Permissions = { ..., Scopes.OpenId, Scopes.Profile },
    Requirements = { Requirements.Features.ProofKeyForCodeExchange }
});
```

---

## 🛠 CHECKLIST CHO Ecommerce.API

- **Cài gói**:
```bash
dotnet add package OpenIddict.Validation.AspNetCore
```

- **Program.cs**:
```csharp
services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("https://localhost:5001");
        options.UseIntrospection()
               .SetClientId("ecommerce_api")
               .SetClientSecret("secret");
        options.UseAspNetCore();
    });
```

- Thêm `[Authorize]` vào Controller:
```csharp
[Authorize]
public class OrdersController : ControllerBase
```

---

## 🧠 GỢI Ý QUẢN LÝ TOKEN TRÊN CLIENT

| Client | Cách lưu token | Cách gọi API |
|--------|----------------|----------------|
| Razor (CustomerApp) | Cookie + `SaveTokens = true` | `HttpContext.GetTokenAsync(...)` |
| React (AdminApp)    | `oidc-client-ts` → lưu vào memory | `fetch('/api/xyz', Authorization: Bearer)` |

---

## ✅ BONUS: API `/api/me` trong AuthServer

```csharp
[Authorize]
[HttpGet("/api/me")]
public IActionResult GetMe()
{
    var userId = User.FindFirst("sub")?.Value;
    return Ok(new { userId });
}
```

---

## 🚀 KẾT LUẬN

- ✅ Tách AuthServer giúp hệ thống scale tốt hơn, dễ bảo mật hơn
- ✅ OpenIddict hỗ trợ OAuth2 + OIDC đúng chuẩn, dễ mở rộng
- ✅ Frontend (React, Razor, Mobile) chỉ cần nhận token, không cần xử lý xác thực trực tiếp

Bạn đã sẵn sàng tích hợp toàn bộ hệ thống ecommerce của mình đúng chuẩn rồi đó 😎

