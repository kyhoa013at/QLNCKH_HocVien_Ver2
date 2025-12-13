# 🚀 Phase 1 - Hướng dẫn Cài đặt

## ✅ Các thay đổi đã thực hiện

### 1. Bảo mật Connection String
- Đã xóa connection string khỏi `appsettings.json`
- Cần cấu hình qua User Secrets (xem hướng dẫn bên dưới)

### 2. Sửa Bug
- ✅ Sửa lỗi filter trong `QuanLySinhVien.razor` (thiếu dấu ngoặc)
- ✅ Sửa lỗi inject IJSRuntime trong `DangKyChuyenDe.razor`
- ✅ Sửa API endpoint typo: `soloa` → `soloai`

### 3. Server-side Validation
- ✅ Thêm validation cho tất cả Controllers
- ✅ Kiểm tra dữ liệu trùng lặp (MaSV, MaSoCB, MaSoCD)
- ✅ Kiểm tra foreign key tồn tại
- ✅ Sanitize input (trim, uppercase mã)
- ✅ Thay thế Raw SQL bằng LINQ an toàn

### 4. Authentication
- ✅ Cookie-based Authentication
- ✅ Trang Login với UI đẹp
- ✅ Hiển thị user info trên header
- ✅ Nút đăng xuất
- ✅ Bảo vệ tất cả API endpoints với `[Authorize]`
- ✅ Trang Access Denied

---

## 📋 Hướng dẫn cài đặt

### Bước 1: Cấu hình Connection String (User Secrets)

Mở terminal tại thư mục `QLNCKH_HocVien/QLNCKH_HocVien` và chạy:

```powershell
# Windows PowerShell
cd QLNCKH_HocVien/QLNCKH_HocVien

# Thiết lập connection string (thay YOUR_PASSWORD bằng mật khẩu thực)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=103.77.243.84,1433;Database=QuanLyNCKH_Db;User Id=sa;Password=YOUR_PASSWORD;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=True;"
```

### Bước 2: Tạo Migration mới cho bảng NguoiDung

```powershell
# Tạo migration
dotnet ef migrations add AddNguoiDungTable

# Cập nhật database
dotnet ef database update
```

### Bước 3: Tạo tài khoản Admin

Chạy script SQL để tạo user admin:

```sql
-- Chạy trong SQL Server Management Studio hoặc Azure Data Studio
-- Password mặc định: Admin@123

INSERT INTO NguoiDungs (TenDangNhap, MatKhau, HoTen, VaiTro, IsActive, NgayTao)
VALUES (
    'admin',
    'o8dJmVSf+0e3kEpbSfbF0DP3lmfKGTG8FhCXJ0kbJbY=',
    N'Quản trị viên',
    'Admin',
    1,
    GETDATE()
);
```

Hoặc chạy file: `Migrations/SeedAdminUser.sql`

### Bước 4: Chạy ứng dụng

```powershell
dotnet run
```

---

## 🔐 Thông tin đăng nhập mặc định

| Tài khoản | Mật khẩu | Vai trò |
|-----------|----------|---------|
| admin | Admin@123 | Admin |

**⚠️ Quan trọng:** Đổi mật khẩu sau khi đăng nhập lần đầu!

---

## 📁 Files đã thay đổi

### Controllers (thêm validation + [Authorize])
- `Controllers/SinhVienController.cs`
- `Controllers/GiaoVienController.cs`
- `Controllers/ChuyenDeNCKHController.cs`
- `Controllers/NopSanPhamController.cs`
- `Controllers/HoiDongController.cs`
- `Controllers/KetQuaController.cs`
- `Controllers/XepGiaiController.cs`
- `Controllers/AuthController.cs` (MỚI)

### Models
- `Models/AuthModels.cs` (MỚI)

### Services
- `Services/AuthService.cs` (MỚI)
- `Services/KetQuaService.cs` (sửa endpoint)

### Pages
- `Pages/Login.razor` (MỚI)
- `Pages/AccessDenied.razor` (MỚI)
- `Pages/QuanLySinhVien.razor` (sửa bug)
- `Pages/DangKyChuyenDe.razor` (sửa bug)

### Data & Config
- `Data/ApplicationDbContext.cs` (thêm NguoiDung + indexes)
- `Program.cs` (thêm Auth middleware)
- `appsettings.json` (xóa connection string)
- `Components/Layout/MainLayout.razor` (thêm user info)

### Helpers
- `Helpers/ValidationHelper.cs` (MỚI)

---

## 🔄 Các bước tiếp theo (Phase 2)

1. Chuẩn hóa API response với wrapper
2. Thêm pagination thực sự
3. Cấu hình EF Core relationships
4. Thêm indexes cho performance

