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
    public class GiaoVienController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GiaoVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GiaoVien (Lấy tất cả)
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<GiaoVien>>>> GetAll()
        {
            var data = await _context.GiaoViens.ToListAsync();
            return this.OkResult(data, $"Lấy {data.Count} giáo viên");
        }

        // GET: api/GiaoVien/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PaginatedResult<GiaoVien>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.GiaoViens.AsQueryable();

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

            return this.PaginatedOk(items, totalCount, pageNumber, pageSize);
        }

        // GET: api/GiaoVien/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<GiaoVien>>> GetById(int id)
        {
            if (id <= 0)
                return this.BadRequestResult<GiaoVien>("ID không hợp lệ");

            var gv = await _context.GiaoViens.FindAsync(id);
            if (gv == null)
                return this.NotFoundResult<GiaoVien>("Không tìm thấy giáo viên");

            return this.OkResult(gv);
        }

        // POST: api/GiaoVien
        [HttpPost]
        public async Task<ActionResult<ApiResult<GiaoVien>>> Create(GiaoVien gv)
        {
            var (isValid, errors) = ValidationHelper.ValidateModel(gv);
            if (!isValid)
                return this.BadRequestResult<GiaoVien>("Dữ liệu không hợp lệ", errors);

            if (await _context.GiaoViens.AnyAsync(x => x.MaSoCB == gv.MaSoCB))
                return this.BadRequestResult<GiaoVien>("Mã số cán bộ đã tồn tại!");

            gv.MaSoCB = gv.MaSoCB.Trim().ToUpper();
            gv.HoTen = gv.HoTen.Trim();

            _context.GiaoViens.Add(gv);
            await _context.SaveChangesAsync();
            return this.CreatedResult(gv, "Thêm giáo viên thành công");
        }

        // PUT: api/GiaoVien/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> Update(int id, GiaoVien gv)
        {
            if (id <= 0)
                return this.BadRequestResult("ID không hợp lệ");
            if (id != gv.Id)
                return this.BadRequestResult("ID không khớp");

            var (isValid, errors) = ValidationHelper.ValidateModel(gv);
            if (!isValid)
                return this.BadRequestResult("Dữ liệu không hợp lệ", errors);

            var existing = await _context.GiaoViens.FindAsync(id);
            if (existing == null)
                return this.NotFoundResult("Không tìm thấy giáo viên");

            if (await _context.GiaoViens.AnyAsync(x => x.MaSoCB == gv.MaSoCB && x.Id != id))
                return this.BadRequestResult("Mã số cán bộ đã tồn tại!");

            gv.MaSoCB = gv.MaSoCB.Trim().ToUpper();
            gv.HoTen = gv.HoTen.Trim();
            _context.Entry(existing).CurrentValues.SetValues(gv);

            await _context.SaveChangesAsync();
            return this.OkResult("Cập nhật giáo viên thành công");
        }

        // DELETE: api/GiaoVien/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            if (id <= 0)
                return this.BadRequestResult("ID không hợp lệ");

            var gv = await _context.GiaoViens.FindAsync(id);
            if (gv == null)
                return this.NotFoundResult("Không tìm thấy giáo viên");

            var inHoiDong = await _context.ThanhVienHoiDongs.AnyAsync(x => x.IdGiaoVien == id);
            if (inHoiDong)
                return this.BadRequestResult("Không thể xóa giáo viên đang tham gia hội đồng chấm!");

            _context.GiaoViens.Remove(gv);
            await _context.SaveChangesAsync();
            return this.OkResult("Xóa giáo viên thành công");
        }
    }
}
