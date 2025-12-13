using QLNCKH_HocVien.Client.Models;

namespace QLNCKH_HocVien.Services
{
    /// <summary>
    /// Service interface cho SinhVien business logic
    /// </summary>
    public interface ISinhVienService
    {
        Task<ApiResult<List<SinhVien>>> GetAllAsync();
        Task<PaginatedResult<SinhVien>> GetPagedAsync(int pageNumber, int pageSize, string? search = null);
        Task<ApiResult<SinhVien>> GetByIdAsync(int id);
        Task<ApiResult<SinhVien>> CreateAsync(SinhVien sv);
        Task<ApiResult> UpdateAsync(int id, SinhVien sv);
        Task<ApiResult> DeleteAsync(int id);
    }

    /// <summary>
    /// Service interface cho GiaoVien business logic
    /// </summary>
    public interface IGiaoVienService
    {
        Task<ApiResult<List<GiaoVien>>> GetAllAsync();
        Task<PaginatedResult<GiaoVien>> GetPagedAsync(int pageNumber, int pageSize, string? search = null);
        Task<ApiResult<GiaoVien>> GetByIdAsync(int id);
        Task<ApiResult<GiaoVien>> CreateAsync(GiaoVien gv);
        Task<ApiResult> UpdateAsync(int id, GiaoVien gv);
        Task<ApiResult> DeleteAsync(int id);
    }

    /// <summary>
    /// Service interface cho ChuyenDe business logic
    /// </summary>
    public interface IChuyenDeService
    {
        Task<ApiResult<List<ChuyenDeNCKH>>> GetAllAsync();
        Task<PaginatedResult<ChuyenDeNCKH>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, int? idLinhVuc = null);
        Task<ApiResult<ChuyenDeNCKH>> GetByIdAsync(int id);
        Task<ApiResult<ChuyenDeNCKH>> CreateAsync(ChuyenDeNCKH cd);
        Task<ApiResult> UpdateAsync(int id, ChuyenDeNCKH cd);
        Task<ApiResult> DeleteAsync(int id);
    }

    /// <summary>
    /// Service interface cho NopSanPham business logic
    /// </summary>
    public interface INopSanPhamService
    {
        Task<ApiResult<List<NopSanPham>>> GetAllAsync();
        Task<PaginatedResult<NopSanPham>> GetPagedAsync(int pageNumber, int pageSize, int? idChuyenDe = null, TrangThaiNop? trangThai = null);
        Task<ApiResult<List<NopSanPham>>> GetByChuyenDeAsync(int idChuyenDe);
        Task<ApiResult<NopSanPham>> CreateAsync(NopSanPham item);
        Task<ApiResult> DeleteAsync(int id);
    }

    /// <summary>
    /// Service interface cho HoiDong business logic
    /// </summary>
    public interface IHoiDongService
    {
        Task<ApiResult<List<HoiDong>>> GetAllAsync();
        Task<PaginatedResult<HoiDong>> GetPagedAsync(int pageNumber, int pageSize, VongCham? vongThi = null);
        Task<ApiResult<List<HoiDong>>> GetByChuyenDeAsync(int idChuyenDe);
        Task<ApiResult<HoiDong>> GetByIdAsync(int id);
        Task<ApiResult<HoiDong>> CreateAsync(HoiDong hd);
        Task<ApiResult> UpdateAsync(int id, HoiDong hd);
        Task<ApiResult> DeleteAsync(int id);
    }

    /// <summary>
    /// Service interface cho KetQua business logic
    /// </summary>
    public interface IKetQuaService
    {
        // Sơ loại
        Task<ApiResult<List<KetQuaSoLoai>>> GetAllSoLoaiAsync();
        Task<PaginatedResult<KetQuaSoLoai>> GetSoLoaiPagedAsync(int pageNumber, int pageSize, bool? ketQua = null);
        Task<ApiResult<KetQuaSoLoai>> GetSoLoaiByChuyenDeAsync(int idChuyenDe);
        Task<ApiResult> SaveSoLoaiAsync(KetQuaSoLoai item);
        Task<ApiResult<int>> AutoTop15Async();

        // Phiếu chấm
        Task<ApiResult<List<PhieuCham>>> GetAllPhieuChamAsync();
        Task<PaginatedResult<PhieuCham>> GetPhieuChamPagedAsync(int pageNumber, int pageSize, int? idChuyenDe = null, int? idGiaoVien = null);
        Task<ApiResult<List<PhieuCham>>> GetPhieuChamByChuyenDeAsync(int idChuyenDe);
        Task<ApiResult> SavePhieuChamAsync(PhieuCham item);
        Task<ApiResult> DeletePhieuChamAsync(int id);
    }

    /// <summary>
    /// Service interface cho XepGiai business logic
    /// </summary>
    public interface IXepGiaiService
    {
        Task<ApiResult<List<XepGiai>>> GetAllAsync();
        Task<PaginatedResult<XepGiai>> GetPagedAsync(int pageNumber, int pageSize, string? tenGiai = null);
        Task<ApiResult<XepGiai>> GetByIdAsync(int id);
        Task<ApiResult<XepGiai>> GetByChuyenDeAsync(int idChuyenDe);
        Task<ApiResult<object>> GetSummaryAsync();
        Task<ApiResult<List<XepGiai>>> ProcessRankingAsync();
        Task<ApiResult> ResetAsync();
    }

    /// <summary>
    /// Service interface cho Auth business logic
    /// </summary>
    public interface IAuthService
    {
        Task<ApiResult<NguoiDung>> ValidateUserAsync(string tenDangNhap, string matKhau);
        Task<ApiResult<NguoiDung>> GetUserByIdAsync(int id);
        Task<ApiResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<ApiResult> CreateUserAsync(NguoiDung user);
        Task<ApiResult> InitAdminAsync();
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }

    /// <summary>
    /// Service interface cho Caching
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
    }
}

