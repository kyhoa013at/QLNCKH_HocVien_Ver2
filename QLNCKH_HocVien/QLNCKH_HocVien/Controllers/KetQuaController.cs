using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Data;
using QLNCKH_HocVien.Helpers;

namespace QLNCKH_HocVien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KetQuaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KetQuaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== VÒNG SƠ LOẠI ====================

        // GET: api/KetQua/soloai-all
        [HttpGet("soloai-all")]
        public async Task<ActionResult<ApiResult<List<KetQuaSoLoai>>>> GetSoLoai()
        {
            var data = await _context.KetQuaSoLoais.ToListAsync();
            return this.OkResult(data, $"Lấy {data.Count} kết quả sơ loại");
        }

        // GET: api/KetQua/soloai/paged
        [HttpGet("soloai/paged")]
        public async Task<ActionResult<PaginatedResult<KetQuaSoLoai>>> GetSoLoaiPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? ketQua = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.KetQuaSoLoais.AsQueryable();

            if (ketQua.HasValue)
            {
                query = query.Where(k => k.KetQua == ketQua);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(k => k.DiemSo)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return this.PaginatedOk(items, totalCount, pageNumber, pageSize);
        }

        // GET: api/KetQua/soloai/5
        [HttpGet("soloai/{idChuyenDe}")]
        public async Task<ActionResult<ApiResult<KetQuaSoLoai>>> GetSoLoaiByChuyenDe(int idChuyenDe)
        {
            if (idChuyenDe <= 0)
                return this.BadRequestResult<KetQuaSoLoai>("ID chuyên đề không hợp lệ");

            var kq = await _context.KetQuaSoLoais.FirstOrDefaultAsync(x => x.IdChuyenDe == idChuyenDe);
            if (kq == null)
                return this.NotFoundResult<KetQuaSoLoai>("Chưa có kết quả sơ loại cho chuyên đề này");

            return this.OkResult(kq);
        }

        // POST: api/KetQua/soloai
        [HttpPost("soloai")]
        public async Task<ActionResult<ApiResult>> SaveSoLoai(KetQuaSoLoai item)
        {
            if (item.IdChuyenDe <= 0)
                return this.BadRequestResult("Chuyên đề không hợp lệ");

            if (!ValidationHelper.IsValidScore(item.DiemSo))
                return this.BadRequestResult("Điểm phải từ 0 đến 10");

            if (!await _context.ChuyenDeNCKHs.AnyAsync(x => x.Id == item.IdChuyenDe))
                return this.BadRequestResult("Chuyên đề không tồn tại");

            var exists = await _context.KetQuaSoLoais.FirstOrDefaultAsync(x => x.IdChuyenDe == item.IdChuyenDe);
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

            await _context.SaveChangesAsync();
            return this.OkResult("Lưu kết quả sơ loại thành công");
        }

        // POST: api/KetQua/auto-top15
        [HttpPost("auto-top15")]
        public async Task<ActionResult<ApiResult<int>>> AutoTop15()
        {
            // 1. Reset toàn bộ về False
            await _context.KetQuaSoLoais.ExecuteUpdateAsync(s => s.SetProperty(x => x.KetQua, false));

            // 2. Lấy danh sách điểm kèm thông tin Lĩnh vực
            var listDiem = await (from kq in _context.KetQuaSoLoais
                                  join cd in _context.ChuyenDeNCKHs on kq.IdChuyenDe equals cd.Id
                                  select new { kq, cd.IdLinhVuc }).ToListAsync();

            // 3. Group theo lĩnh vực và lấy Top 15
            var listPass = listDiem
                .GroupBy(x => x.IdLinhVuc)
                .SelectMany(g => g.OrderByDescending(x => x.kq.DiemSo).Take(15))
                .Select(x => x.kq)
                .ToList();

            // 4. Update trạng thái
            foreach (var item in listPass) item.KetQua = true;
            await _context.SaveChangesAsync();

            return this.OkResult(listPass.Count, $"Đã xét duyệt Top 15 thành công! ({listPass.Count} chuyên đề)");
        }

        // ==================== VÒNG CHUNG KHẢO (PHIẾU CHẤM) ====================

        // GET: api/KetQua/phieucham-all
        [HttpGet("phieucham-all")]
        public async Task<ActionResult<ApiResult<List<PhieuCham>>>> GetAllPhieuCham()
        {
            var data = await _context.PhieuChams.ToListAsync();
            return this.OkResult(data, $"Lấy {data.Count} phiếu chấm");
        }

        // GET: api/KetQua/phieucham/paged
        [HttpGet("phieucham/paged")]
        public async Task<ActionResult<PaginatedResult<PhieuCham>>> GetPhieuChamPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? idChuyenDe = null,
            [FromQuery] int? idGiaoVien = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.PhieuChams.AsQueryable();

            if (idChuyenDe.HasValue && idChuyenDe > 0)
            {
                query = query.Where(p => p.IdChuyenDe == idChuyenDe);
            }

            if (idGiaoVien.HasValue && idGiaoVien > 0)
            {
                query = query.Where(p => p.IdGiaoVien == idGiaoVien);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.Diem)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return this.PaginatedOk(items, totalCount, pageNumber, pageSize);
        }

        // GET: api/KetQua/phieucham/5
        [HttpGet("phieucham/{idChuyenDe}")]
        public async Task<ActionResult<ApiResult<List<PhieuCham>>>> GetPhieuCham(int idChuyenDe)
        {
            if (idChuyenDe <= 0)
                return this.BadRequestResult<List<PhieuCham>>("ID chuyên đề không hợp lệ");

            var data = await _context.PhieuChams
                .Where(x => x.IdChuyenDe == idChuyenDe)
                .ToListAsync();

            return this.OkResult(data);
        }

        // POST: api/KetQua/phieucham
        [HttpPost("phieucham")]
        public async Task<ActionResult<ApiResult>> SavePhieuCham(PhieuCham pc)
        {
            if (pc.IdChuyenDe <= 0)
                return this.BadRequestResult("Chuyên đề không hợp lệ");

            if (pc.IdGiaoVien <= 0)
                return this.BadRequestResult("Giáo viên không hợp lệ");

            if (!ValidationHelper.IsValidScore(pc.Diem))
                return this.BadRequestResult("Điểm phải từ 0 đến 10");

            if (!await _context.ChuyenDeNCKHs.AnyAsync(x => x.Id == pc.IdChuyenDe))
                return this.BadRequestResult("Chuyên đề không tồn tại");

            if (!await _context.GiaoViens.AnyAsync(x => x.Id == pc.IdGiaoVien))
                return this.BadRequestResult("Giáo viên không tồn tại");

            var exists = await _context.PhieuChams
                .FirstOrDefaultAsync(x => x.IdChuyenDe == pc.IdChuyenDe && x.IdGiaoVien == pc.IdGiaoVien);

            if (exists != null)
            {
                exists.Diem = pc.Diem;
                exists.YKien = pc.YKien?.Trim();
            }
            else
            {
                pc.YKien = pc.YKien?.Trim();
                _context.PhieuChams.Add(pc);
            }

            await _context.SaveChangesAsync();
            return this.OkResult("Lưu phiếu chấm thành công");
        }

        // DELETE: api/KetQua/phieucham/5
        [HttpDelete("phieucham/{id}")]
        public async Task<ActionResult<ApiResult>> DeletePhieuCham(int id)
        {
            if (id <= 0)
                return this.BadRequestResult("ID không hợp lệ");

            var pc = await _context.PhieuChams.FindAsync(id);
            if (pc == null)
                return this.NotFoundResult("Không tìm thấy phiếu chấm");

            _context.PhieuChams.Remove(pc);
            await _context.SaveChangesAsync();
            return this.OkResult("Xóa phiếu chấm thành công");
        }
    }
}
