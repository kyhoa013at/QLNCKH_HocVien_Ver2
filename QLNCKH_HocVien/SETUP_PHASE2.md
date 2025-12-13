# 🚀 Phase 2 - API & Database Improvements

## ✅ Các thay đổi đã thực hiện

### 1. Chuẩn hóa API Response với Wrapper

**File mới:** `Models/ApiResult.cs`

Tất cả API endpoints giờ trả về định dạng chuẩn:

```json
// ApiResult<T> - Response có data
{
    "success": true,
    "message": "Thành công",
    "data": { ... },
    "errors": []
}

// ApiResult - Response không có data
{
    "success": true,
    "message": "Cập nhật thành công",
    "errors": []
}

// PaginatedResult<T> - Response có phân trang
{
    "success": true,
    "message": "Thành công",
    "data": [...],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10,
    "hasPreviousPage": false,
    "hasNextPage": true
}
```

### 2. Pagination API

Mỗi entity có thêm endpoint phân trang:

| Entity | Endpoint | Query Params |
|--------|----------|--------------|
| SinhVien | `GET /api/SinhVien/paged` | `pageNumber`, `pageSize`, `search` |
| GiaoVien | `GET /api/GiaoVien/paged` | `pageNumber`, `pageSize`, `search` |
| ChuyenDeNCKH | `GET /api/ChuyenDeNCKH/paged` | `pageNumber`, `pageSize`, `search`, `idLinhVuc` |
| NopSanPham | `GET /api/NopSanPham/paged` | `pageNumber`, `pageSize`, `idChuyenDe`, `trangThai` |
| HoiDong | `GET /api/HoiDong/paged` | `pageNumber`, `pageSize`, `vongThi` |
| KetQuaSoLoai | `GET /api/KetQua/soloai/paged` | `pageNumber`, `pageSize`, `ketQua` |
| PhieuCham | `GET /api/KetQua/phieucham/paged` | `pageNumber`, `pageSize`, `idChuyenDe`, `idGiaoVien` |
| XepGiai | `GET /api/XepGiai/paged` | `pageNumber`, `pageSize`, `tenGiai` |

**Ví dụ sử dụng:**
```
GET /api/SinhVien/paged?pageNumber=1&pageSize=10&search=nguyen
```

### 3. EF Core Relationships đầy đủ

**File cập nhật:** `Data/ApplicationDbContext.cs`

#### Relationships được cấu hình:

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│    SinhVien     │◄────│  ChuyenDeNCKH   │────▶│    GiaoVien     │
│  (One)          │     │   (Many)        │     │  (Restrict)     │
└─────────────────┘     └────────┬────────┘     └─────────────────┘
                                 │
        ┌────────────────────────┼────────────────────────┐
        ▼ (Cascade)              ▼ (Cascade)              ▼ (Cascade)
┌───────────────┐      ┌─────────────────┐      ┌─────────────────┐
│  NopSanPham   │      │    HoiDong      │      │    XepGiai      │
└───────────────┘      └────────┬────────┘      └─────────────────┘
                                │ (Cascade)
                       ┌────────┴────────┐
                       ▼                 ▼
              ┌────────────────┐  ┌─────────────────┐
              │ThanhVienHoiDong│  │  KetQuaSoLoai   │
              │  (Restrict GV) │  └─────────────────┘
              └────────────────┘          │
                                   ┌──────┴───────┐
                                   ▼              
                           ┌─────────────────┐     
                           │   PhieuCham     │     
                           │ (Restrict GV)   │    
                           └─────────────────┘     
```

#### Delete Behaviors:
- **Cascade**: Xóa ChuyenDe → Tự động xóa NopSanPham, HoiDong, KetQuaSoLoai, PhieuCham, XepGiai
- **Restrict**: Không thể xóa GiaoVien nếu đang tham gia HoiDong hoặc có PhieuCham

### 4. Indexes cho Performance

| Bảng | Index | Unique |
|------|-------|--------|
| NguoiDung | TenDangNhap | ✅ |
| SinhVien | MaSV | ✅ |
| GiaoVien | MaSoCB | ✅ |
| ChuyenDeNCKH | MaSoCD | ✅ |
| ChuyenDeNCKH | IdLinhVuc, IdHocVien | |
| NopSanPham | IdChuyenDe, TrangThai, NgayNop | |
| HoiDong | IdChuyenDe + VongThi | ✅ |
| HoiDong | NgayCham | |
| ThanhVienHoiDong | IdHoiDong + IdGiaoVien | ✅ |
| KetQuaSoLoai | IdChuyenDe | ✅ |
| KetQuaSoLoai | KetQua, DiemSo | |
| PhieuCham | IdChuyenDe + IdGiaoVien | ✅ |
| XepGiai | IdChuyenDe | ✅ |
| XepGiai | XepHang, TenGiai | |

### 5. API Endpoints Mới

#### SinhVien
- `GET /api/SinhVien/{id}` - Lấy theo ID

#### GiaoVien  
- `GET /api/GiaoVien/{id}` - Lấy theo ID

#### ChuyenDeNCKH
- `GET /api/ChuyenDeNCKH/{id}` - Lấy theo ID
- `PUT /api/ChuyenDeNCKH/{id}` - Cập nhật

#### NopSanPham
- `GET /api/NopSanPham/by-chuyende/{id}` - Lấy theo chuyên đề

#### HoiDong
- `GET /api/HoiDong/{id}` - Lấy theo ID
- `GET /api/HoiDong/by-chuyende/{id}` - Lấy theo chuyên đề
- `PUT /api/HoiDong/{id}` - Cập nhật

#### KetQua
- `GET /api/KetQua/soloai/{idChuyenDe}` - Lấy kết quả sơ loại theo chuyên đề
- `DELETE /api/KetQua/phieucham/{id}` - Xóa phiếu chấm

#### XepGiai
- `GET /api/XepGiai/{id}` - Lấy theo ID
- `GET /api/XepGiai/by-chuyende/{id}` - Lấy theo chuyên đề
- `GET /api/XepGiai/summary` - Thống kê theo giải
- `DELETE /api/XepGiai/reset` - Reset toàn bộ xếp giải

---

## 📋 Hướng dẫn cập nhật

### Bước 1: Tạo Migration mới

```powershell
cd QLNCKH_HocVien/QLNCKH_HocVien

# Tạo migration Phase 2
dotnet ef migrations add Phase2_ApiAndRelationships

# Xem script SQL (tùy chọn)
dotnet ef migrations script --idempotent -o phase2_migration.sql

# Cập nhật database
dotnet ef database update
```

### Bước 2: Build và Test

```powershell
# Build project
dotnet build

# Chạy ứng dụng
dotnet run
```

### Bước 3: Test API với Pagination

```powershell
# Test pagination
curl "https://localhost:5001/api/SinhVien/paged?pageNumber=1&pageSize=5"

# Test với search
curl "https://localhost:5001/api/SinhVien/paged?search=nguyen"
```

---

## 📁 Files đã thay đổi

### Models (Client)
- `Models/ApiResult.cs` (MỚI) - Wrapper classes cho API response

### Controllers (Server)
- `Controllers/SinhVienController.cs` - Thêm pagination, chuẩn hóa response
- `Controllers/GiaoVienController.cs` - Thêm pagination, chuẩn hóa response
- `Controllers/ChuyenDeNCKHController.cs` - Thêm pagination, PUT endpoint
- `Controllers/NopSanPhamController.cs` - Thêm pagination, filter by chuyên đề
- `Controllers/HoiDongController.cs` - Thêm pagination, PUT endpoint
- `Controllers/KetQuaController.cs` - Thêm pagination, DELETE endpoint
- `Controllers/XepGiaiController.cs` - Thêm pagination, summary, reset

### Helpers (Server)
- `Helpers/ApiResultExtensions.cs` (MỚI) - Extension methods cho Controller

### Data (Server)
- `Data/ApplicationDbContext.cs` - Cấu hình relationships và indexes

### Services (Client)
- `Services/SinhVienService.cs` - Hỗ trợ ApiResult và pagination
- `Services/GiaoVienService.cs` - Hỗ trợ ApiResult và pagination
- `Services/ChuyenDeNCKH.cs` - Hỗ trợ ApiResult và pagination
- `Services/NopSanPhamService.cs` - Hỗ trợ ApiResult và pagination
- `Services/HoiDongService.cs` - Hỗ trợ ApiResult và pagination
- `Services/KetQuaService.cs` - Hỗ trợ ApiResult và pagination
- `Services/XepGiaiService.cs` - Hỗ trợ ApiResult và pagination

### Extensions (Client)
- `Extensions/HttpClientExtensions.cs` - Thêm PostAsyncWithAuth, PutAsJsonAsyncWithAuth

---

## 🔄 Breaking Changes

### API Response Format
Các API cũ trả về trực tiếp data:
```json
[{ "id": 1, "maSV": "SV001", ... }]
```

Giờ trả về wrapper:
```json
{
    "success": true,
    "message": "Lấy 10 sinh viên",
    "data": [{ "id": 1, "maSV": "SV001", ... }],
    "errors": []
}
```

### Client Services
Các method cũ như `ThemSinhVien()` giờ trả về `ApiResult<SinhVien>` thay vì `void/Task`.

**Cách dùng mới:**
```csharp
var result = await SinhVienService.ThemSinhVien(sv);
if (result.Success)
{
    // Thành công
    var newSv = result.Data;
}
else
{
    // Lỗi
    var errorMessage = result.Message;
}
```

---

## 🔄 Các bước tiếp theo (Phase 3)

1. **Architecture**
   - Tạo interfaces cho services (IRepository pattern)
   - Tách business logic ra services riêng
   - Dependency Injection chuẩn

2. **Caching**
   - Memory Cache cho danh mục
   - Distributed Cache (Redis) nếu scale

3. **Logging**
   - Serilog cho structured logging
   - Log vào file và console
   - Correlation IDs

4. **Error Handling**
   - Global Exception Handler
   - ProblemDetails format
   - Retry policies

---

## 📝 Notes

- Pagination mặc định: pageSize = 10, max = 100
- Search không phân biệt hoa/thường (case-insensitive)
- Các API GET cũ vẫn hoạt động (backward compatible)
- Indexes sẽ được tạo khi chạy migration


