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
    public class HoiDongController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HoiDongController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/HoiDong
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<HoiDong>>>> GetAll()
        {
            var data = await _context.HoiDongs
                .Include(h => h.ThanhViens)
                .ToListAsync();
            return this.OkResult(data, $"Lấy {data.Count} hội đồng");
        }

        // GET: api/HoiDong/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PaginatedResult<HoiDong>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] VongCham? vongThi = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.HoiDongs
                .Include(h => h.ThanhViens)
                .AsQueryable();

            if (vongThi.HasValue)
            {
                query = query.Where(h => h.VongThi == vongThi);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(h => h.NgayCham)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return this.PaginatedOk(items, totalCount, pageNumber, pageSize);
        }

        // GET: api/HoiDong/by-chuyende/5
        [HttpGet("by-chuyende/{idChuyenDe}")]
        public async Task<ActionResult<ApiResult<List<HoiDong>>>> GetByChuyenDe(int idChuyenDe)
        {
            if (idChuyenDe <= 0)
                return this.BadRequestResult<List<HoiDong>>("ID chuyên đề không hợp lệ");

            var data = await _context.HoiDongs
                .Include(h => h.ThanhViens)
                .Where(h => h.IdChuyenDe == idChuyenDe)
                .ToListAsync();

            return this.OkResult(data);
        }

        // GET: api/HoiDong/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<HoiDong>>> GetById(int id)
        {
            if (id <= 0)
                return this.BadRequestResult<HoiDong>("ID không hợp lệ");

            var hd = await _context.HoiDongs
                .Include(h => h.ThanhViens)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hd == null)
                return this.NotFoundResult<HoiDong>("Không tìm thấy hội đồng");

            return this.OkResult(hd);
        }

        // POST: api/HoiDong
        [HttpPost]
        public async Task<ActionResult<ApiResult<HoiDong>>> Create(HoiDong hd)
        {
            if (hd.IdChuyenDe <= 0)
                return this.BadRequestResult<HoiDong>("Vui lòng chọn chuyên đề!");

            if (!await _context.ChuyenDeNCKHs.AnyAsync(x => x.Id == hd.IdChuyenDe))
                return this.BadRequestResult<HoiDong>("Chuyên đề không tồn tại!");

            var exists = await _context.HoiDongs.AnyAsync(x => x.IdChuyenDe == hd.IdChuyenDe && x.VongThi == hd.VongThi);
            if (exists)
                return this.BadRequestResult<HoiDong>($"Chuyên đề này đã có hội đồng chấm {hd.VongThi} rồi!");

            // Validate thành viên
            if (hd.ThanhViens != null && hd.ThanhViens.Any())
            {
                var gvIds = hd.ThanhViens.Select(t => t.IdGiaoVien).ToList();
                var existingGvCount = await _context.GiaoViens.CountAsync(g => gvIds.Contains(g.Id));
                if (existingGvCount != gvIds.Count)
                    return this.BadRequestResult<HoiDong>("Một hoặc nhiều giáo viên không tồn tại!");
            }

            hd.DiaDiem = hd.DiaDiem?.Trim();

            _context.HoiDongs.Add(hd);
            await _context.SaveChangesAsync();
            return this.CreatedResult(hd, "Tạo hội đồng thành công");
        }

        // PUT: api/HoiDong/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> Update(int id, HoiDong hd)
        {
            if (id <= 0)
                return this.BadRequestResult("ID không hợp lệ");
            if (id != hd.Id)
                return this.BadRequestResult("ID không khớp");

            var existing = await _context.HoiDongs
                .Include(h => h.ThanhViens)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (existing == null)
                return this.NotFoundResult("Không tìm thấy hội đồng");

            // Cập nhật thông tin cơ bản
            existing.NgayCham = hd.NgayCham;
            existing.DiaDiem = hd.DiaDiem?.Trim();
            existing.VongThi = hd.VongThi;

            // Cập nhật thành viên: xóa cũ, thêm mới
            if (existing.ThanhViens != null)
            {
                _context.ThanhVienHoiDongs.RemoveRange(existing.ThanhViens);
            }

            if (hd.ThanhViens != null && hd.ThanhViens.Any())
            {
                foreach (var tv in hd.ThanhViens)
                {
                    tv.IdHoiDong = id;
                    _context.ThanhVienHoiDongs.Add(tv);
                }
            }

            await _context.SaveChangesAsync();
            return this.OkResult("Cập nhật hội đồng thành công");
        }

        // DELETE: api/HoiDong/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            if (id <= 0)
                return this.BadRequestResult("ID không hợp lệ");

            var hd = await _context.HoiDongs
                .Include(x => x.ThanhViens)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (hd == null)
                return this.NotFoundResult("Không tìm thấy hội đồng");

            if (hd.ThanhViens != null && hd.ThanhViens.Any())
            {
                _context.ThanhVienHoiDongs.RemoveRange(hd.ThanhViens);
            }

            _context.HoiDongs.Remove(hd);
            await _context.SaveChangesAsync();

            return this.OkResult("Xóa hội đồng thành công");
        }
    }
}
