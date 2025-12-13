using Microsoft.EntityFrameworkCore;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Data;

namespace QLNCKH_HocVien.Repositories
{
    // ==================== SINH VIÊN ====================
    public class SinhVienRepository : Repository<SinhVien>, ISinhVienRepository
    {
        public SinhVienRepository(ApplicationDbContext context) : base(context) { }

        public async Task<SinhVien?> GetByMaSVAsync(string maSV)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.MaSV == maSV);
        }

        public async Task<bool> IsMaSVExistsAsync(string maSV, int? excludeId = null)
        {
            return excludeId.HasValue
                ? await _dbSet.AnyAsync(s => s.MaSV == maSV && s.Id != excludeId)
                : await _dbSet.AnyAsync(s => s.MaSV == maSV);
        }

        public async Task<(IEnumerable<SinhVien> Items, int TotalCount)> SearchAsync(
            int pageNumber, int pageSize, string? search = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(s =>
                    s.MaSV.ToLower().Contains(search) ||
                    s.HoTen.ToLower().Contains(search) ||
                    (s.Lop != null && s.Lop.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.MaSV)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }

    // ==================== GIÁO VIÊN ====================
    public class GiaoVienRepository : Repository<GiaoVien>, IGiaoVienRepository
    {
        public GiaoVienRepository(ApplicationDbContext context) : base(context) { }

        public async Task<GiaoVien?> GetByMaSoCBAsync(string maSoCB)
        {
            return await _dbSet.FirstOrDefaultAsync(g => g.MaSoCB == maSoCB);
        }

        public async Task<bool> IsMaSoCBExistsAsync(string maSoCB, int? excludeId = null)
        {
            return excludeId.HasValue
                ? await _dbSet.AnyAsync(g => g.MaSoCB == maSoCB && g.Id != excludeId)
                : await _dbSet.AnyAsync(g => g.MaSoCB == maSoCB);
        }

        public async Task<bool> IsInHoiDongAsync(int id)
        {
            return await _context.ThanhVienHoiDongs.AnyAsync(t => t.IdGiaoVien == id);
        }

        public async Task<(IEnumerable<GiaoVien> Items, int TotalCount)> SearchAsync(
            int pageNumber, int pageSize, string? search = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(g =>
                    g.MaSoCB.ToLower().Contains(search) ||
                    g.HoTen.ToLower().Contains(search) ||
                    (g.LinhVuc != null && g.LinhVuc.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(g => g.MaSoCB)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }

    // ==================== CHUYÊN ĐỀ ====================
    public class ChuyenDeRepository : Repository<ChuyenDeNCKH>, IChuyenDeRepository
    {
        public ChuyenDeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ChuyenDeNCKH?> GetByMaSoCDAsync(string maSoCD)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.MaSoCD == maSoCD);
        }

        public async Task<bool> IsMaSoCDExistsAsync(string maSoCD, int? excludeId = null)
        {
            return excludeId.HasValue
                ? await _dbSet.AnyAsync(c => c.MaSoCD == maSoCD && c.Id != excludeId)
                : await _dbSet.AnyAsync(c => c.MaSoCD == maSoCD);
        }

        public async Task<(IEnumerable<ChuyenDeNCKH> Items, int TotalCount)> SearchAsync(
            int pageNumber, int pageSize, string? search = null, int? idLinhVuc = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(c =>
                    c.MaSoCD.ToLower().Contains(search) ||
                    c.TenChuyenDe.ToLower().Contains(search));
            }

            if (idLinhVuc.HasValue && idLinhVuc > 0)
            {
                query = query.Where(c => c.IdLinhVuc == idLinhVuc);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.MaSoCD)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task DeleteWithRelatedDataAsync(int id)
        {
            await _context.XepGiais.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();
            await _context.PhieuChams.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();
            await _context.KetQuaSoLoais.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();
            await _context.NopSanPhams.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();

            var hoiDongIds = await _context.HoiDongs
                .Where(x => x.IdChuyenDe == id)
                .Select(x => x.Id)
                .ToListAsync();
            await _context.ThanhVienHoiDongs
                .Where(x => hoiDongIds.Contains(x.IdHoiDong))
                .ExecuteDeleteAsync();
            await _context.HoiDongs.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();

            var cd = await _dbSet.FindAsync(id);
            if (cd != null)
            {
                _dbSet.Remove(cd);
            }
        }
    }

    // ==================== NỘP SẢN PHẨM ====================
    public class NopSanPhamRepository : Repository<NopSanPham>, INopSanPhamRepository
    {
        public NopSanPhamRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<NopSanPham>> GetByChuyenDeAsync(int idChuyenDe)
        {
            return await _dbSet
                .Where(n => n.IdChuyenDe == idChuyenDe)
                .OrderByDescending(n => n.NgayNop)
                .ToListAsync();
        }

        public async Task<bool> IsAlreadySubmittedAsync(int idChuyenDe, TrangThaiNop trangThai)
        {
            return await _dbSet.AnyAsync(n => n.IdChuyenDe == idChuyenDe && n.TrangThai == trangThai);
        }
    }

    // ==================== HỘI ĐỒNG ====================
    public class HoiDongRepository : Repository<HoiDong>, IHoiDongRepository
    {
        public HoiDongRepository(ApplicationDbContext context) : base(context) { }

        public async Task<HoiDong?> GetByIdWithMembersAsync(int id)
        {
            return await _dbSet
                .Include(h => h.ThanhViens)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<HoiDong>> GetAllWithMembersAsync()
        {
            return await _dbSet.Include(h => h.ThanhViens).ToListAsync();
        }

        public async Task<IEnumerable<HoiDong>> GetByChuyenDeAsync(int idChuyenDe)
        {
            return await _dbSet
                .Include(h => h.ThanhViens)
                .Where(h => h.IdChuyenDe == idChuyenDe)
                .ToListAsync();
        }

        public async Task<bool> ExistsForChuyenDeAndVongAsync(int idChuyenDe, VongCham vongThi)
        {
            return await _dbSet.AnyAsync(h => h.IdChuyenDe == idChuyenDe && h.VongThi == vongThi);
        }

        public async Task DeleteWithMembersAsync(int id)
        {
            var hd = await GetByIdWithMembersAsync(id);
            if (hd != null)
            {
                if (hd.ThanhViens != null && hd.ThanhViens.Any())
                {
                    _context.ThanhVienHoiDongs.RemoveRange(hd.ThanhViens);
                }
                _dbSet.Remove(hd);
            }
        }
    }

    // ==================== KẾT QUẢ ====================
    public class KetQuaRepository : IKetQuaRepository
    {
        private readonly ApplicationDbContext _context;

        public KetQuaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sơ loại
        public async Task<IEnumerable<KetQuaSoLoai>> GetAllSoLoaiAsync()
        {
            return await _context.KetQuaSoLoais.ToListAsync();
        }

        public async Task<KetQuaSoLoai?> GetSoLoaiByChuyenDeAsync(int idChuyenDe)
        {
            return await _context.KetQuaSoLoais.FirstOrDefaultAsync(k => k.IdChuyenDe == idChuyenDe);
        }

        public async Task SaveSoLoaiAsync(KetQuaSoLoai item)
        {
            var exists = await _context.KetQuaSoLoais.FirstOrDefaultAsync(k => k.IdChuyenDe == item.IdChuyenDe);
            if (exists != null)
            {
                exists.DiemSo = item.DiemSo;
                exists.KetQua = item.KetQua;
                exists.NhanXet = item.NhanXet?.Trim();
            }
            else
            {
                item.NhanXet = item.NhanXet?.Trim();
                _context.KetQuaSoLoais.Add(item);
            }
        }

        public async Task<int> AutoTop15Async()
        {
            await _context.KetQuaSoLoais.ExecuteUpdateAsync(s => s.SetProperty(x => x.KetQua, false));

            var listDiem = await (from kq in _context.KetQuaSoLoais
                                  join cd in _context.ChuyenDeNCKHs on kq.IdChuyenDe equals cd.Id
                                  select new { kq, cd.IdLinhVuc }).ToListAsync();

            var listPass = listDiem
                .GroupBy(x => x.IdLinhVuc)
                .SelectMany(g => g.OrderByDescending(x => x.kq.DiemSo).Take(15))
                .Select(x => x.kq)
                .ToList();

            foreach (var item in listPass) item.KetQua = true;
            await _context.SaveChangesAsync();

            return listPass.Count;
        }

        // Phiếu chấm
        public async Task<IEnumerable<PhieuCham>> GetAllPhieuChamAsync()
        {
            return await _context.PhieuChams.ToListAsync();
        }

        public async Task<IEnumerable<PhieuCham>> GetPhieuChamByChuyenDeAsync(int idChuyenDe)
        {
            return await _context.PhieuChams.Where(p => p.IdChuyenDe == idChuyenDe).ToListAsync();
        }

        public async Task SavePhieuChamAsync(PhieuCham item)
        {
            var exists = await _context.PhieuChams
                .FirstOrDefaultAsync(p => p.IdChuyenDe == item.IdChuyenDe && p.IdGiaoVien == item.IdGiaoVien);

            if (exists != null)
            {
                exists.Diem = item.Diem;
                exists.YKien = item.YKien?.Trim();
            }
            else
            {
                item.YKien = item.YKien?.Trim();
                _context.PhieuChams.Add(item);
            }
        }

        public async Task DeletePhieuChamAsync(int id)
        {
            var pc = await _context.PhieuChams.FindAsync(id);
            if (pc != null)
            {
                _context.PhieuChams.Remove(pc);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }

    // ==================== XẾP GIẢI ====================
    public class XepGiaiRepository : Repository<XepGiai>, IXepGiaiRepository
    {
        public XepGiaiRepository(ApplicationDbContext context) : base(context) { }

        public async Task<XepGiai?> GetByChuyenDeAsync(int idChuyenDe)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.IdChuyenDe == idChuyenDe);
        }

        public async Task<IEnumerable<object>> GetSummaryAsync()
        {
            return await _dbSet
                .GroupBy(x => x.TenGiai)
                .Select(g => new { TenGiai = g.Key, SoLuong = g.Count() })
                .OrderBy(x => x.TenGiai)
                .ToListAsync<object>();
        }

        public async Task<IEnumerable<XepGiai>> ProcessRankingAsync()
        {
            _context.XepGiais.RemoveRange(_context.XepGiais);
            await _context.SaveChangesAsync();

            var listDiem = await _context.PhieuChams
                .GroupBy(pc => pc.IdChuyenDe)
                .Select(g => new
                {
                    IdChuyenDe = g.Key,
                    DiemTB = g.Average(pc => pc.Diem)
                })
                .OrderByDescending(x => x.DiemTB)
                .ToListAsync();

            var danhSachGiai = new List<XepGiai>();
            int rank = 1;

            foreach (var item in listDiem)
            {
                var giai = new XepGiai
                {
                    IdChuyenDe = item.IdChuyenDe,
                    DiemTrungBinh = Math.Round(item.DiemTB, 2),
                    XepHang = rank
                };

                if (rank <= 3) giai.TenGiai = "Giải Nhất";
                else if (rank <= 8) giai.TenGiai = "Giải Nhì";
                else if (rank <= 15) giai.TenGiai = "Giải Ba";
                else if (rank <= 25) giai.TenGiai = "Giải Khuyến Khích";
                else giai.TenGiai = "Công nhận NCKH";

                danhSachGiai.Add(giai);
                rank++;
            }

            _context.XepGiais.AddRange(danhSachGiai);
            await _context.SaveChangesAsync();

            return danhSachGiai;
        }

        public async Task ResetAsync()
        {
            _context.XepGiais.RemoveRange(_context.XepGiais);
            await _context.SaveChangesAsync();
        }
    }

    // ==================== NGƯỜI DÙNG ====================
    public class NguoiDungRepository : Repository<NguoiDung>, INguoiDungRepository
    {
        public NguoiDungRepository(ApplicationDbContext context) : base(context) { }

        public async Task<NguoiDung?> GetByTenDangNhapAsync(string tenDangNhap)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap && u.IsActive);
        }

        public async Task<bool> IsTenDangNhapExistsAsync(string tenDangNhap, int? excludeId = null)
        {
            return excludeId.HasValue
                ? await _dbSet.AnyAsync(u => u.TenDangNhap == tenDangNhap && u.Id != excludeId)
                : await _dbSet.AnyAsync(u => u.TenDangNhap == tenDangNhap);
        }
    }
}

