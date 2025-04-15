# ⚙️ LUỒNG XỬ LÝ AUTHENTICATION

## 🧭 LUỒNG 1: ĐĂNG KÝ (Local Account)

### Flow:

1. Người dùng điền form đăng ký: email + mật khẩu + tên + sđt...
2. Server gọi `_userManager.CreateAsync()` tạo user mới.
3. Lưu vào:
   - **AspNetUsers**: thông tin email, password hash.
   - **CustomerProfiles**: thông tin FullName, Phone, Address,...
   - _(Option)_ **AspNetUserClaims**: `"is_verified": false`.

### 💾 Database tương tác:

| **Bảng**             | **Vai trò**                                     |
| -------------------- | ----------------------------------------------- |
| **AspNetUsers**      | Tạo user mới                                    |
| **CustomerProfiles** | Tạo hồ sơ người dùng                            |
| **AspNetUserClaims** | Gắn các đặc tính (claim) như tier, verified,... |

---

## 🔐 LUỒNG 2: ĐĂNG NHẬP (Local Login)

### Flow:

1. Người dùng gửi email + mật khẩu.
2. Server kiểm tra hợp lệ → đăng nhập thành công.
3. Server dùng OpenIddict để cấp:
   - `access_token`
   - `refresh_token`
   - Các claim như email, role, tier, is_verified sẽ được đưa vào token (lấy từ DB).

### 💾 Database tương tác:

| **Bảng**                                                     | **Dùng để**                  |
| ------------------------------------------------------------ | ---------------------------- |
| **AspNetUsers**                                              | So khớp user/password        |
| **AspNetUserClaims**                                         | Đưa claim vào token          |
| **AspNetUserRoles** + **AspNetRoles** + **AspNetRoleClaims** | Phân quyền và cấp quyền động |
| **AspNetUserTokens**                                         | Lưu `refresh_token` nếu cần  |

---

## 🧭 LUỒNG 3: ĐĂNG NHẬP BẰNG GOOGLE

### Flow:

1. Người dùng click “Sign in with Google” → Redirect tới Google.
2. Google xác thực → redirect lại về Auth Server.
3. Auth Server:
   - Nếu user chưa tồn tại → tạo user mới trong **AspNetUsers**.
   - Lưu vào **AspNetUserLogins**:
     - `LoginProvider = Google`
     - `ProviderKey = <google_id>`
   - Cấp `access_token` như login bình thường.
   - Tạo **CustomerProfile** nếu chưa có.

### 💾 Database tương tác:

| **Bảng**             | **Dùng để**                        |
| -------------------- | ---------------------------------- |
| **AspNetUsers**      | Tạo user mới                       |
| **AspNetUserLogins** | Lưu login bằng Google              |
| **CustomerProfiles** | Tạo hồ sơ cá nhân mới              |
| **AspNetUserClaims** | Có thể lưu email, avatar từ Google |

---

## 🧠 LUỒNG 4: CẤP TOKEN (OAuth2 + PKCE)

1. Khi user login xong → Auth Server cấp `authorization_code`.
2. App frontend (React, Razor) gửi `POST /connect/token`.
3. Auth Server xác minh → trả về:
   - `access_token` (thường là JWT).
   - `refresh_token` (lưu vào **AspNetUserTokens**).
4. Token này sẽ được client dùng để gọi các API.

---

## 🔁 LUỒNG 5: REFRESH TOKEN

1. Khi `access_token` hết hạn → frontend dùng `refresh_token` gửi lên `/connect/token`.
2. Auth Server tìm `refresh_token` trong **AspNetUserTokens**, xác minh.
3. Nếu hợp lệ → cấp `access_token` mới.

### 💾 Database tương tác:

| **Bảng**             | **Vai trò**                    |
| -------------------- | ------------------------------ |
| **AspNetUserTokens** | Lưu và tra cứu `refresh_token` |

---

## 🧑‍💻 LUỒNG 6: TRUY CẬP API SAU KHI LOGIN

1. Client gọi API: `GET /api/products`.
2. Gửi kèm `Authorization: Bearer <access_token>`.
3. Middleware JWT xác minh token hợp lệ.
4. API kiểm tra các claim trong token:
   - Nếu có `role=admin` → cho phép thêm sản phẩm.
   - Nếu có `is_verified=true` → cho phép đặt hàng.
5. ➡️ Không cần truy vấn DB nữa, vì tất cả nằm trong claim đã cấp trong token!

---

## ✏️ LUỒNG 7: CHỈNH SỬA HỒ SƠ

1. Người dùng chỉnh sửa thông tin như tên, số điện thoại, địa chỉ.
2. Server gọi bảng **CustomerProfiles** để cập nhật record theo `UserId`.
3. Không tác động đến bảng **AspNetUsers** nếu không có thay đổi về email hoặc mật khẩu.

---

## ✅ Tổng hợp bảng dùng theo chức năng

| **Chức năng**           | **Bảng tương tác chính**                                        |
| ----------------------- | --------------------------------------------------------------- |
| **Đăng ký (local)**     | **AspNetUsers**, **CustomerProfiles**, **AspNetUserClaims**     |
| **Đăng nhập (local)**   | **AspNetUsers**, **AspNetUserClaims**, **AspNetUserRoles**      |
| **Đăng nhập (Google)**  | **AspNetUsers**, **AspNetUserLogins**, **CustomerProfiles**     |
| **Cấp `access_token`**  | **AspNetUserClaims**, **AspNetUserRoles**, **AspNetRoleClaims** |
| **Cấp `refresh_token`** | **AspNetUserTokens**                                            |
| **Gọi API bằng token**  | Không cần query DB – đọc từ claim                               |
| **Cập nhật hồ sơ**      | **CustomerProfiles**                                            |
