# QLNCKH_HocVien

## 📋 Tổng quan

Hệ thống quản lý nghiên cứu khoa học học viên, xây dựng bằng **ASP.NET Core 8.0** với kiến trúc **Blazor** (Server + WebAssembly).

## 🏗️ Kiến trúc

### 1. Server Project (QLNCKH_HocVien)
- ASP.NET Core Web App với Blazor Server
- Entity Framework Core với SQL Server
- API Controllers (REST)
- MudBlazor UI Framework

### 2. Client Project (QLNCKH_HocVien.Client)
- Blazor WebAssembly
- Models, Services, Pages
- Gọi API server và API danh mục ngoài

## ✨ Chức năng chính

### 1. Quản lý danh mục
- **Quản lý Sinh viên**: CRUD, xuất Excel, lọc
- **Quản lý Giáo viên**: thêm, sửa, xóa thông tin giáo viên

### 2. Quản lý chuyên đề NCKH
- **Đăng ký chuyên đề**: Sinh viên đăng ký đề tài nghiên cứu
- **Nộp sản phẩm**: Nộp file/tài liệu sản phẩm nghiên cứu

### 3. Đánh giá & chấm điểm
- **Lập hội đồng**: Tạo hội đồng chấm (vòng sơ loại/chung khảo)
- **Chấm điểm**:
  - Vòng sơ loại: 1 người chấm → `KetQuaSoLoai`
  - Vòng chung khảo: nhiều người chấm → `PhieuCham`
- **Xếp giải**: Xếp hạng và trao giải cho các đề tài

## 🗄️ Cơ sở dữ liệu

### Các bảng chính

| Bảng | Mô tả |
|------|-------|
| `SinhVien` | Thông tin học viên |
| `GiaoVien` | Thông tin giáo viên |
| `ChuyenDeNCKH` | Chuyên đề nghiên cứu |
| `NopSanPham` | Sản phẩm nộp |
| `HoiDong` | Hội đồng chấm |
| `ThanhVienHoiDong` | Thành viên hội đồng |
| `KetQuaSoLoai` | Kết quả vòng sơ loại |
| `PhieuCham` | Phiếu chấm vòng chung khảo |
| `XepGiai` | Kết quả xếp giải |

## 🔄 Luồng hoạt động

```
1. Sinh viên đăng ký chuyên đề → ChuyenDeNCKH
2. Nộp sản phẩm → NopSanPham
3. Tạo hội đồng chấm → HoiDong + ThanhVienHoiDong
4. Chấm sơ loại → KetQuaSoLoai
5. Chấm chung khảo → PhieuCham
6. Xếp giải → XepGiai
```

## 🛠️ Công nghệ & thư viện

- **.NET 8.0**
- **Blazor** (Server + WebAssembly)
- **Entity Framework Core 8.0**
- **SQL Server**
- **MudBlazor 8.15.0** - UI Components
- **ClosedXML** - Xuất Excel
- **Bootstrap 5** - Responsive layout

## 📁 Cấu trúc thư mục

```
QLNCKH_HocVien/
├── QLNCKH_HocVien/          # Server project
│   ├── Controllers/         # API Controllers
│   ├── Data/               # DbContext
│   ├── Migrations/         # EF Core migrations
│   ├── Components/         # Blazor components
│   └── Program.cs
│
└── QLNCKH_HocVien.Client/   # Client project
    ├── Models/             # Data models
    ├── Services/           # API service classes
    ├── Pages/              # Blazor pages
    └── Program.cs
```

## ✅ Điểm mạnh

1. ✨ Kiến trúc tách Server/Client rõ ràng
2. 🔄 Sử dụng EF Core Migrations
3. 🎨 UI có animation và validation
4. 📊 Xuất Excel
5. 🔗 Tích hợp API danh mục ngoài (`apidanhmuc.6pg.org`)
6. 🔍 Hỗ trợ lọc/tìm kiếm

## ⚠️ Điểm cần cải thiện

1. 🔐 **Bảo mật Connection String**: Mật khẩu trong `appsettings.json` → Nên dùng User Secrets hoặc Azure Key Vault
2. 🔒 **Thiếu Authentication/Authorization**: Cần thêm xác thực và phân quyền
3. 📝 **Logging/Error Handling**: Chưa có logging và xử lý lỗi toàn diện
4. 📄 **Pagination**: UI pagination chưa hoạt động (chỉ hiển thị, chưa phân trang thực)

## 🚀 Cài đặt & Chạy

### Yêu cầu

- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 hoặc VS Code

### Các bước

1. Clone repository
```bash
git clone [repository-url]
cd QLNCKH_HocVien
```

2. Cập nhật Connection String trong `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=QLNCKH_HocVien;..."
  }
}
```

3. Chạy Migrations
```bash
cd QLNCKH_HocVien
dotnet ef database update
```

4. Chạy ứng dụng
```bash
dotnet run
```

## 📝 License

[...]

## 👥 Đóng góp

[cahoa05]

## 📧 Liên hệ

[https://www.facebook.com/cahoa05]
