using QLNCKH_HocVien.Client.Models;

namespace QLNCKH_HocVien.Repositories
{
    /// <summary>
    /// Repository cho SinhVien với các method đặc thù
    /// </summary>
    public interface ISinhVienRepository : IRepository<SinhVien>
    {
        Task<SinhVien?> GetByMaSVAsync(string maSV);
        Task<bool> IsMaSVExistsAsync(string maSV, int? excludeId = null);
        Task<(IEnumerable<SinhVien> Items, int TotalCount)> SearchAsync(
            int pageNumber, int pageSize, string? search = null);
    }

    /// <summary>
    /// Repository cho GiaoVien với các method đặc thù
    /// </summary>
    public interface IGiaoVienRepository : IRepository<GiaoVien>
    {
        Task<GiaoVien?> GetByMaSoCBAsync(string maSoCB);
        Task<bool> IsMaSoCBExistsAsync(string maSoCB, int? excludeId = null);
        Task<bool> IsInHoiDongAsync(int id);
        Task<(IEnumerable<GiaoVien> Items, int TotalCount)> SearchAsync(
            int pageNumber, int pageSize, string? search = null);
    }

    /// <summary>
    /// Repository cho ChuyenDeNCKH với các method đặc thù
    /// </summary>
    public interface IChuyenDeRepository : IRepository<ChuyenDeNCKH>
    {
        Task<ChuyenDeNCKH?> GetByMaSoCDAsync(string maSoCD);
        Task<bool> IsMaSoCDExistsAsync(string maSoCD, int? excludeId = null);
        Task<(IEnumerable<ChuyenDeNCKH> Items, int TotalCount)> SearchAsync(
            int pageNumber, int pageSize, string? search = null, int? idLinhVuc = null);
        Task DeleteWithRelatedDataAsync(int id);
    }

    /// <summary>
    /// Repository cho NopSanPham
    /// </summary>
    public interface INopSanPhamRepository : IRepository<NopSanPham>
    {
        Task<IEnumerable<NopSanPham>> GetByChuyenDeAsync(int idChuyenDe);
        Task<bool> IsAlreadySubmittedAsync(int idChuyenDe, TrangThaiNop trangThai);
    }

    /// <summary>
    /// Repository cho HoiDong
    /// </summary>
    public interface IHoiDongRepository : IRepository<HoiDong>
    {
        Task<HoiDong?> GetByIdWithMembersAsync(int id);
        Task<IEnumerable<HoiDong>> GetAllWithMembersAsync();
        Task<IEnumerable<HoiDong>> GetByChuyenDeAsync(int idChuyenDe);
        Task<bool> ExistsForChuyenDeAndVongAsync(int idChuyenDe, VongCham vongThi);
        Task DeleteWithMembersAsync(int id);
    }

    /// <summary>
    /// Repository cho KetQua (KetQuaSoLoai + PhieuCham)
    /// </summary>
    public interface IKetQuaRepository
    {
        // Sơ loại
        Task<IEnumerable<KetQuaSoLoai>> GetAllSoLoaiAsync();
        Task<KetQuaSoLoai?> GetSoLoaiByChuyenDeAsync(int idChuyenDe);
        Task SaveSoLoaiAsync(KetQuaSoLoai item);
        Task<int> AutoTop15Async();

        // Phiếu chấm
        Task<IEnumerable<PhieuCham>> GetAllPhieuChamAsync();
        Task<IEnumerable<PhieuCham>> GetPhieuChamByChuyenDeAsync(int idChuyenDe);
        Task SavePhieuChamAsync(PhieuCham item);
        Task DeletePhieuChamAsync(int id);

        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// Repository cho XepGiai
    /// </summary>
    public interface IXepGiaiRepository : IRepository<XepGiai>
    {
        Task<XepGiai?> GetByChuyenDeAsync(int idChuyenDe);
        Task<IEnumerable<object>> GetSummaryAsync();
        Task<IEnumerable<XepGiai>> ProcessRankingAsync();
        Task ResetAsync();
    }

    /// <summary>
    /// Repository cho NguoiDung
    /// </summary>
    public interface INguoiDungRepository : IRepository<NguoiDung>
    {
        Task<NguoiDung?> GetByTenDangNhapAsync(string tenDangNhap);
        Task<bool> IsTenDangNhapExistsAsync(string tenDangNhap, int? excludeId = null);
    }
}

