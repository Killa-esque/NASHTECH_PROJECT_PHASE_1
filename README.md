# EcommerceSolution

## 🧠 TỔNG QUAN VỀ CÁC TẦNG (LAYER)

Clean Architecture chia hệ thống thành nhiều tầng, với nguyên lý:

**"Phụ thuộc hướng vào trong" (Dependency Rule):** Tầng trong không được biết tầng ngoài, tầng ngoài phụ thuộc vào tầng trong.

---

## 🔍 CÁC TẦNG CHUẨN TRONG DỰ ÁN EcommerceSolution

### 🟩 1. Domain Layer – “Trái tim” của hệ thống

Chứa các Entity, Interface, Enum, Business Rule thô sơ.

📂 **Ecommerce.Domain** gồm:

- `Product.cs`, `Category.cs`
- `IProductRepository.cs`, `IUnitOfWork.cs`
- Các Enum như `UserRole`

✅ **Không phụ thuộc ai hết** → mọi tầng khác đều có thể dùng domain.

---

### 🟦 2. Application Layer – "Luật chơi" cụ thể

Chứa các Service, Use Case, định nghĩa các nghiệp vụ cụ thể bằng các interface từ domain.

📂 **Ecommerce.Application** gồm:

- `IProductService`, `ProductService`
- `ProductDTO`, `CategoryDTO`
- Giao tiếp qua interface `IProductRepository`

✅ **Phụ thuộc Domain**, nhưng không phụ thuộc Infrastructure → Dễ test vì dùng DI + Mock interface.

---

### 🟧 3. Infrastructure Layer – “Cái máy làm việc thực tế”

Chứa code cụ thể: EF Core, gửi mail, đọc file, cache, gọi API ngoài…

📂 **Ecommerce.Infrastructure** gồm:

- `EfProductRepository.cs` implement `IProductRepository`
- `AppDbContext.cs`
- `AuthService.cs`

✅ **Phụ thuộc Application và Domain** → Là nơi implement cụ thể, dễ thay thế.

---

### 🟥 4. API Layer (Presentation) – “Mặt tiền” của backend

Chứa Controller, Swagger, JWT Auth, gọi Service từ Application.

📂 **Ecommerce.API** gồm:

- `ProductController`, `CategoryController`
- Gọi `IProductService`, `ICategoryService`
- Register Service, Repository vào DI container

✅ **Là lớp ngoài cùng** → gọi đến Application.

---

### 🔄 5. CustomerApp (MVC) & AdminApp (React)

Đây là giao diện người dùng, gọi API hoặc Razor view để hiển thị.

| **CustomerApp**         | **AdminApp**     |
| ----------------------- | ---------------- |
| Gọi API hoặc Razor View | Gọi API từ React |

**Luồng phụ thuộc:**
CustomerApp → [API Layer] → [Application Layer] → [Domain Layer] ← [Infrastructure Layer]

---

## 🔁 LUỒNG NGHIỆP VỤ

### 🔁 1. LUỒNG KHÁCH HÀNG (CustomerApp - Razor Pages)

👉 **Khi khách truy cập trang chủ:**

1. Trình duyệt gọi: `GET /`
2. Razor Controller gọi `ProductService.GetFeaturedProducts()`
3. `ProductService` (Application layer) gọi `IProductRepository`
4. `EfProductRepository` (Infrastructure) truy vấn DB qua `AppDbContext`
5. Dữ liệu trả về → render Razor View
6. HTML trả về trình duyệt.

👉 **Đăng ký / Đăng nhập:**

1. Gửi form `POST` đến controller.
2. Xác thực bằng `AuthService` trong Application layer.
3. Nếu đúng → tạo Cookie auth hoặc token (tùy thiết kế).

---

### 🔁 2. LUỒNG ADMIN (AdminApp - React)

👉 **Khi admin vào trang quản lý sản phẩm:**

1. React gọi API: `GET /api/products`.
2. Request tới `Ecommerce.API` → `ProductController`.
3. Controller gọi `ProductService`.
4. Service gọi `ProductRepository` (qua interface).
5. EF Core lấy từ DB → map sang DTO → trả về JSON.
6. React hiển thị danh sách sản phẩm.

👉 **Khi tạo sản phẩm mới:**

1. React gửi `POST /api/products` + form data.
2. API nhận → gọi `ProductService.CreateProduct()`.
3. Service gọi `repository.Add()` → `SaveChanges()`.
4. Trả về JSON → hiển thị thông báo thành công.

---

### 🔁 3. Xác thực (Authentication Flow)

- **Tùy chọn:** sử dụng JWT hoặc Cookie (tuỳ phân hệ).
  - **AdminApp** nên dùng JWT.
  - **CustomerApp** có thể dùng Cookie + ASP.NET Identity.
- 🔐 Nếu tích hợp OpenIddict / IdentityServer4, API sẽ xác thực token theo chuẩn OAuth2 → chuyên nghiệp hơn.

<!-- dotnet ef dbcontext scaffold "Server=localhost,1433;Database=SweetCakeShopDB;User Id=sa;Password=Mayhabuoi@123;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer --context AppDbContext --output-dir src/Ecommerce.Infrastructure/Data/Entities --context-dir src/Ecommerce.Infrastructure/Data --namespace Ecommerce.Infrastructure.Data.Entities --context-namespace Ecommerce.Infrastructure.Data --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.API --force    -->

<!-- +------------------+          +---------------------+         +------------------+
| Customer Site    |          | AuthorizationServer |         | Resource API     |
| (Razor Pages)    | <======> | ASP.NET Core + OIDC | <=====> | ASP.NET Web API  |
| + PKCE flow      |          | (OpenIddict)        |         | [Authorize]      |
+------------------+          +---------------------+         +------------------+
        |
        | [1] redirect đến /connect/authorize
        |
        v
+-----------------------------+
| Người dùng đăng nhập        |
+-----------------------------+
        |
        v
[2] Trả về "code" (auth code) thông qua redirect_uri
        |
        v
[3] Customer Site gửi code + code_verifier đến /connect/token
        |
        v
[4] Nhận access_token, id_token, refresh_token
        |
        v
[5] Gọi API (Product, Order...) với access_token trong header         -->


