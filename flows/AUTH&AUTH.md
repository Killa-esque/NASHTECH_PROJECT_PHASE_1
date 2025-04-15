# 🔐 TÓM TẮT LUỒNG ĐĂNG NHẬP – CUSTOMER SITE (OAuth2 + PKCE + OpenIddict)

## ⚙️ Thành phần chính trong hệ thống

| **Thành phần**          | **Công nghệ**                  | **Vai trò**                                      |
|--------------------------|--------------------------------|-------------------------------------------------|
| **Authorization Server** | ASP.NET Core Web API + OpenIddict | Xác thực người dùng, cấp token                  |
| **Customer Site**        | ASP.NET Core MVC (Razor Pages) | Ứng dụng frontend, gửi yêu cầu login, nhận token |
| **Resource Server (API)**| ASP.NET Core Web API          | Cung cấp dữ liệu, yêu cầu access_token để truy cập |

---

## 🔁 Luồng xử lý chi tiết

1. **Người dùng nhấn nút Đăng nhập tại Customer Site**  
   - Customer Site (MVC) redirect người dùng sang Authorization Server (`/connect/authorize`)  
   - Gửi kèm:
     - `client_id=customer_site`
     - `redirect_uri=https://customer.com/signin-oidc`
     - `response_type=code`
     - `code_challenge` (dùng cho PKCE)
     - `scope=openid profile email`

2. **Authorization Server (OpenIddict) xử lý**  
   - Hiển thị form đăng nhập  
   - Người dùng nhập email + password → xác thực thành công  

3. **Authorization Server cấp authorization_code**  
   - Redirect ngược về `redirect_uri` của Customer Site  
   - Gửi kèm code tạm thời  

4. **Customer Site gọi `/connect/token` (POST)**  
   - Dùng `code` + `code_verifier` (PKCE)  
   - Gửi đến: `https://auth-server.com/connect/token`  
   - Nhận về:
     - `access_token`: dùng để gọi API
     - `refresh_token`: dùng để lấy token mới sau này  

5. **Customer Site lưu token, hiển thị nội dung**  
   - Gọi API qua `Authorization: Bearer <access_token>`  
   - Lấy thông tin người dùng, sản phẩm, v.v.  

6. **Khi token hết hạn → dùng refresh_token để lấy mới**  
   - Gửi `refresh_token` lên `/connect/token`  
   - Nhận token mới mà không cần login lại  

---

## 📌 Tóm gọn vai trò theo kiến trúc

| **Thành phần**          | **Vai trò cụ thể**                                   |
|--------------------------|-----------------------------------------------------|
| **Authorization Server** | Cài OpenIddict. Xử lý login, cấp code, cấp access_token |
| **Customer Site (MVC)**  | Chỉ redirect login, nhận access_token, gọi API      |
| **API Server**           | Bảo vệ bằng `[Authorize]`, dùng middleware xác thực token |

---

## ✅ Kết luận cuối cùng cho đại vương

- **Customer Site** không xử lý login, không cấp token. Nó chỉ là một “Client” — ủy quyền cho Authorization Server làm việc đó.  
- **Authorization Server** mới là nơi tiếp nhận `/connect/authorize`, `/connect/token` và xử lý toàn bộ chuẩn OAuth2 + PKCE.
