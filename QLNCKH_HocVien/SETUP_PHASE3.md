# 🚀 Phase 3 - Architecture Improvements

## ✅ Các thay đổi đã thực hiện

### 1. Repository Pattern

**Thư mục:** `Repositories/`

#### Generic Repository Interface
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(...);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}
```

#### Specific Repositories
| Repository | Interface | Đặc thù |
|------------|-----------|---------|
| SinhVienRepository | ISinhVienRepository | `GetByMaSVAsync`, `SearchAsync` |
| GiaoVienRepository | IGiaoVienRepository | `GetByMaSoCBAsync`, `IsInHoiDongAsync` |
| ChuyenDeRepository | IChuyenDeRepository | `DeleteWithRelatedDataAsync` |
| NopSanPhamRepository | INopSanPhamRepository | `GetByChuyenDeAsync` |
| HoiDongRepository | IHoiDongRepository | `GetAllWithMembersAsync` |
| KetQuaRepository | IKetQuaRepository | `AutoTop15Async` |
| XepGiaiRepository | IXepGiaiRepository | `ProcessRankingAsync` |
| NguoiDungRepository | INguoiDungRepository | `GetByTenDangNhapAsync` |

#### Unit of Work Pattern
```csharp
public interface IUnitOfWork : IDisposable
{
    ISinhVienRepository SinhViens { get; }
    IGiaoVienRepository GiaoViens { get; }
    // ... other repositories
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### 2. Business Services Layer

**Thư mục:** `Services/`

#### Service Interfaces
```csharp
public interface ISinhVienService
{
    Task<ApiResult<List<SinhVien>>> GetAllAsync();
    Task<PaginatedResult<SinhVien>> GetPagedAsync(...);
    Task<ApiResult<SinhVien>> GetByIdAsync(int id);
    Task<ApiResult<SinhVien>> CreateAsync(SinhVien sv);
    Task<ApiResult> UpdateAsync(int id, SinhVien sv);
    Task<ApiResult> DeleteAsync(int id);
}
```

#### Implemented Services
- `SinhVienBusinessService` - Logic nghiệp vụ sinh viên
- `GiaoVienBusinessService` - Logic nghiệp vụ giáo viên
- `ChuyenDeBusinessService` - Logic nghiệp vụ chuyên đề

### 3. Memory Cache Service

**File:** `Services/CacheService.cs`

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPrefixAsync(string prefix);
}
```

**Cache Keys:**
```csharp
public static class CacheKeys
{
    public const string AllSinhViens = "sinhviens:all";
    public const string AllGiaoViens = "giaoviens:all";
    // ...
    
    public static string SinhVien(int id) => $"sinhvien:{id}";
    public static string GiaoVien(int id) => $"giaovien:{id}";
    // ...
}
```

### 4. Logging

**Cấu hình trong Program.cs:**
```csharp
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(
    builder.Environment.IsDevelopment() ? LogLevel.Debug : LogLevel.Information);
```

**Request Logging Middleware:**
- Log tất cả API requests với TraceId
- Log response time (elapsed milliseconds)
- Warning level cho status code >= 400

### 5. Global Exception Handler

**File:** `Middleware/ExceptionMiddleware.cs`

**Features:**
- Catch unhandled exceptions
- Log với TraceId để debug
- Trả về `ApiResult` format chuẩn
- Development mode: Include stack trace
- Production mode: Generic error message

**Response format:**
```json
{
    "success": false,
    "message": "Đã xảy ra lỗi. Vui lòng thử lại sau.",
    "errors": ["Stack trace only in Development"]
}
```

**Headers:** `X-Trace-Id` để correlate logs

---

## 📁 Cấu trúc thư mục mới

```
QLNCKH_HocVien/
├── Controllers/           # API Controllers (existing)
├── Data/                  # DbContext (existing)
├── Helpers/               # Utilities (existing)
├── Middleware/            # [NEW] Middlewares
│   ├── ExceptionMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Repositories/          # [NEW] Repository Pattern
│   ├── IRepository.cs
│   ├── ISpecificRepositories.cs
│   ├── Repository.cs
│   ├── SpecificRepositories.cs
│   └── UnitOfWork.cs
├── Services/              # [NEW] Business Services
│   ├── IBusinessServices.cs
│   ├── BusinessServices.cs
│   └── CacheService.cs
├── Components/            # Blazor Components (existing)
├── Migrations/            # EF Migrations (existing)
└── Program.cs             # [UPDATED] DI Configuration
```

---

## 📋 Dependency Injection Registration

**Program.cs đã đăng ký:**

```csharp
// Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
builder.Services.AddScoped<IGiaoVienRepository, GiaoVienRepository>();
// ... other repositories

// Business Services
builder.Services.AddScoped<ISinhVienService, SinhVienBusinessService>();
builder.Services.AddScoped<IGiaoVienService, GiaoVienBusinessService>();
builder.Services.AddScoped<IChuyenDeService, ChuyenDeBusinessService>();
```

---

## 🔄 Cách sử dụng

### Repository Pattern trong Controller

**Trước (trực tiếp DbContext):**
```csharp
public class SinhVienController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public async Task<ActionResult> Get()
    {
        var data = await _context.SinhViens.ToListAsync();
        return Ok(data);
    }
}
```

**Sau (qua Repository):**
```csharp
public class SinhVienController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<ActionResult> Get()
    {
        var data = await _unitOfWork.SinhViens.GetAllAsync();
        return this.OkResult(data.ToList());
    }
}
```

### Business Service trong Controller

```csharp
public class SinhVienController : ControllerBase
{
    private readonly ISinhVienService _sinhVienService;
    
    public async Task<ActionResult<ApiResult<SinhVien>>> Create(SinhVien sv)
    {
        return await _sinhVienService.CreateAsync(sv);
    }
}
```

### Caching

```csharp
public class MyService
{
    private readonly ICacheService _cache;
    
    public async Task<List<SinhVien>> GetAllWithCache()
    {
        var cached = await _cache.GetAsync<List<SinhVien>>(CacheKeys.AllSinhViens);
        if (cached != null) return cached;
        
        var data = await _repository.GetAllAsync();
        await _cache.SetAsync(CacheKeys.AllSinhViens, data.ToList(), TimeSpan.FromMinutes(30));
        return data.ToList();
    }
    
    public async Task InvalidateCache()
    {
        await _cache.RemoveByPrefixAsync(CacheKeys.SinhVienPrefix);
    }
}
```

---

## 🔄 Logging Examples

**Console Output:**
```
info: QLNCKH_HocVien.Middleware.RequestLoggingMiddleware[0]
      API Request: 0HMPQ5H1N:00000001 GET /api/SinhVien started

info: QLNCKH_HocVien.Services.SinhVienBusinessService[0]
      Lấy tất cả sinh viên

info: QLNCKH_HocVien.Middleware.RequestLoggingMiddleware[0]
      API Response: 0HMPQ5H1N:00000001 GET /api/SinhVien responded 200 in 45ms
```

**Error Log:**
```
fail: QLNCKH_HocVien.Middleware.ExceptionMiddleware[0]
      Unhandled exception occurred. TraceId: 0HMPQ5H1N:00000002, Path: /api/SinhVien, Method: POST
      System.InvalidOperationException: Mã sinh viên đã tồn tại!
         at QLNCKH_HocVien.Services.SinhVienBusinessService.CreateAsync(...)
```

---

## 📋 Hướng dẫn cập nhật

### Bước 1: Build và Test

```powershell
cd QLNCKH_HocVien/QLNCKH_HocVien
dotnet build
dotnet run
```

### Bước 2: Test Exception Handler

```powershell
# Thử tạo sinh viên với mã trùng
curl -X POST https://localhost:5001/api/SinhVien \
  -H "Content-Type: application/json" \
  -d '{"maSV":"SV001","hoTen":"Test"}'

# Response sẽ là ApiResult format với error message
```

### Bước 3: Xem Logs

Logs sẽ hiển thị trong Console với format:
- Request/Response với timing
- TraceId để correlate
- Errors với stack trace (Development)

---

## 🔄 Các bước tiếp theo (Phase 4 - UX & Polish)

1. **Pagination UI thực sự**
   - MudTable với server-side pagination
   - Page size selector
   - Navigation buttons

2. **MudBlazor Dialogs & Snackbars**
   - Confirm dialogs cho delete
   - Success/Error snackbars

3. **Loading States**
   - MudSkeleton cho loading
   - MudProgressLinear

4. **Dark Mode**
   - Theme toggle
   - Persist preference

---

## 📝 Notes

- Repository pattern giúp testability tốt hơn (có thể mock)
- Unit of Work đảm bảo transaction consistency
- Cache service dùng Memory Cache (single server)
- Logging sử dụng built-in ILogger (có thể upgrade lên Serilog sau)
- Exception middleware catch tất cả unhandled exceptions


