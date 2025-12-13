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
    public class XepGiaiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public XepGiaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/XepGiai
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<XepGiai>>>> GetAll()
        {
            var data = await _context.XepGiais.OrderBy(x => x.XepHang).ToListAsync();
            return this.OkResult(data, $"Lấy {data.Count} kết quả xếp giải");
        }

        // GET: api/XepGiai/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PaginatedResult<XepGiai>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? tenGiai = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.XepGiais.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tenGiai))
            {
                tenGiai = tenGiai.Trim().ToLower();
                query = query.Where(x => x.TenGiai.ToLower().Contains(tenGiai));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.XepHang)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return this.PaginatedOk(items, totalCount, pageNumber, pageSize);
        }

        // GET: api/XepGiai/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<XepGiai>>> GetById(int id)
        {
            if (id <= 0)
                return this.BadRequestResult<XepGiai>("ID không hợp lệ");

            var xg = await _context.XepGiais.FindAsync(id);
            if (xg == null)
                return this.NotFoundResult<XepGiai>("Không tìm thấy kết quả xếp giải");

            return this.OkResult(xg);
        }

        // GET: api/XepGiai/by-chuyende/5
        [HttpGet("by-chuyende/{idChuyenDe}")]
        public async Task<ActionResult<ApiResult<XepGiai>>> GetByChuyenDe(int idChuyenDe)
        {
            if (idChuyenDe <= 0)
                return this.BadRequestResult<XepGiai>("ID chuyên đề không hợp lệ");

            var xg = await _context.XepGiais.FirstOrDefaultAsync(x => x.IdChuyenDe == idChuyenDe);
            if (xg == null)
                return this.NotFoundResult<XepGiai>("Chuyên đề chưa được xếp giải");

            return this.OkResult(xg);
        }

        // GET: api/XepGiai/summary
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResult<object>>> GetSummary()
        {
            var data = await _context.XepGiais
                .GroupBy(x => x.TenGiai)
                .Select(g => new { TenGiai = g.Key, SoLuong = g.Count() })
                .OrderBy(x => x.TenGiai)
                .ToListAsync();

            return this.OkResult<object>(data, "Thống kê xếp giải");
        }

        // POST: api/XepGiai/process
        [HttpPost("process")]
        public async Task<ActionResult<ApiResult<List<XepGiai>>>> ProcessRanking()
        {
            // 1. Xóa kết quả cũ
            _context.XepGiais.RemoveRange(_context.XepGiais);
            await _context.SaveChangesAsync();

            // 2. Lấy danh sách đề tài và tính điểm TB từ Phiếu chấm
            var listDiem = await _context.PhieuChams
                .GroupBy(pc => pc.IdChuyenDe)
                .Select(g => new
                {
                    IdChuyenDe = g.Key,
                    DiemTB = g.Average(pc => pc.Diem)
                })
                .OrderByDescending(x => x.DiemTB)
                .ToListAsync();

            if (!listDiem.Any())
                return this.BadRequestResult<List<XepGiai>>("Chưa có phiếu chấm nào để xếp giải!");

            // 3. Thuật toán gán giải (3-5-7-10)
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

            // 4. Lưu vào DB
            _context.XepGiais.AddRange(danhSachGiai);
            await _context.SaveChangesAsync();

            return this.OkResult(danhSachGiai, $"Xếp giải thành công cho {danhSachGiai.Count} chuyên đề");
        }

        // DELETE: api/XepGiai/reset
        [HttpDelete("reset")]
        public async Task<ActionResult<ApiResult>> ResetRanking()
        {
            var count = await _context.XepGiais.CountAsync();
            _context.XepGiais.RemoveRange(_context.XepGiais);
            await _context.SaveChangesAsync();

            return this.OkResult($"Đã xóa {count} kết quả xếp giải");
        }
    }
}
