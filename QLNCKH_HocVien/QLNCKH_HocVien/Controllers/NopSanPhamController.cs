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
    public class NopSanPhamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NopSanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/NopSanPham
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<NopSanPham>>>> GetAll()
        {
            var data = await _context.NopSanPhams.ToListAsync();
            return this.OkResult(data, $"Lấy {data.Count} bản nộp");
        }

        // GET: api/NopSanPham/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PaginatedResult<NopSanPham>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? idChuyenDe = null,
            [FromQuery] TrangThaiNop? trangThai = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.NopSanPhams.AsQueryable();

            if (idChuyenDe.HasValue && idChuyenDe > 0)
            {
                query = query.Where(n => n.IdChuyenDe == idChuyenDe);
            }

            if (trangThai.HasValue)
            {
                query = query.Where(n => n.TrangThai == trangThai);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.NgayNop)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return this.PaginatedOk(items, totalCount, pageNumber, pageSize);
        }

        // GET: api/NopSanPham/by-chuyende/5
        [HttpGet("by-chuyende/{idChuyenDe}")]
        public async Task<ActionResult<ApiResult<List<NopSanPham>>>> GetByChuyenDe(int idChuyenDe)
        {
            if (idChuyenDe <= 0)
                return this.BadRequestResult<List<NopSanPham>>("ID chuyên đề không hợp lệ");

            var data = await _context.NopSanPhams
                .Where(n => n.IdChuyenDe == idChuyenDe)
                .OrderByDescending(n => n.NgayNop)
                .ToListAsync();

            return this.OkResult(data);
        }

        // POST: api/NopSanPham
        [HttpPost]
        public async Task<ActionResult<ApiResult<NopSanPham>>> Create(NopSanPham item)
        {
            if (item.IdChuyenDe <= 0)
                return this.BadRequestResult<NopSanPham>("Vui lòng chọn chuyên đề!");

            if (!await _context.ChuyenDeNCKHs.AnyAsync(x => x.Id == item.IdChuyenDe))
                return this.BadRequestResult<NopSanPham>("Chuyên đề không tồn tại!");

            var existingNop = await _context.NopSanPhams
                .AnyAsync(x => x.IdChuyenDe == item.IdChuyenDe && x.TrangThai == item.TrangThai);
            if (existingNop)
                return this.BadRequestResult<NopSanPham>($"Chuyên đề này đã nộp {item.TrangThai} rồi!");

            item.NgayNop = DateTime.Now;
            item.GhiChu = item.GhiChu?.Trim();

            _context.NopSanPhams.Add(item);
            await _context.SaveChangesAsync();
            return this.CreatedResult(item, "Nộp sản phẩm thành công");
        }

        // DELETE: api/NopSanPham/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            if (id <= 0)
                return this.BadRequestResult("ID không hợp lệ");

            var item = await _context.NopSanPhams.FindAsync(id);
            if (item == null)
                return this.NotFoundResult("Không tìm thấy bản nộp");

            _context.NopSanPhams.Remove(item);
            await _context.SaveChangesAsync();
            return this.OkResult("Xóa bản nộp thành công");
        }
    }
}
