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
    public class ChuyenDeNCKHController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChuyenDeNCKHController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ChuyenDeNCKH (Lấy tất cả)
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<ChuyenDeNCKH>>>> GetAll()
        {
            var data = await _context.ChuyenDeNCKHs.ToListAsync();
            return this.OkResult(data, $"Lấy {data.Count} chuyên đề");
        }

        // GET: api/ChuyenDeNCKH/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PaginatedResult<ChuyenDeNCKH>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? idLinhVuc = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.ChuyenDeNCKHs.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(c =>
                    c.MaSoCD.ToLower().Contains(search) ||
                    c.TenChuyenDe.ToLower().Contains(search));
            }

            // Filter by lĩnh vực
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

            return this.PaginatedOk(items, totalCount, pageNumber, pageSize);
        }

        // GET: api/ChuyenDeNCKH/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<ChuyenDeNCKH>>> GetById(int id)
        {
            if (id <= 0)
                return this.BadRequestResult<ChuyenDeNCKH>("ID không hợp lệ");

            var cd = await _context.ChuyenDeNCKHs.FindAsync(id);
            if (cd == null)
                return this.NotFoundResult<ChuyenDeNCKH>("Không tìm thấy chuyên đề");

            return this.OkResult(cd);
        }

        // POST: api/ChuyenDeNCKH
        [HttpPost]
        public async Task<ActionResult<ApiResult<ChuyenDeNCKH>>> Create(ChuyenDeNCKH cd)
        {
            var (isValid, errors) = ValidationHelper.ValidateModel(cd);
            if (!isValid)
                return this.BadRequestResult<ChuyenDeNCKH>("Dữ liệu không hợp lệ", errors);

            if (!await _context.SinhViens.AnyAsync(x => x.Id == cd.IdHocVien))
                return this.BadRequestResult<ChuyenDeNCKH>("Học viên không tồn tại!");

            if (await _context.ChuyenDeNCKHs.AnyAsync(x => x.MaSoCD == cd.MaSoCD))
                return this.BadRequestResult<ChuyenDeNCKH>("Mã chuyên đề đã tồn tại!");

            cd.MaSoCD = cd.MaSoCD.Trim().ToUpper();
            cd.TenChuyenDe = cd.TenChuyenDe.Trim();

            _context.ChuyenDeNCKHs.Add(cd);
            await _context.SaveChangesAsync();
            return this.CreatedResult(cd, "Đăng ký chuyên đề thành công");
        }

        // PUT: api/ChuyenDeNCKH/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> Update(int id, ChuyenDeNCKH cd)
        {
            if (id <= 0)
                return this.BadRequestResult("ID không hợp lệ");
            if (id != cd.Id)
                return this.BadRequestResult("ID không khớp");

            var (isValid, errors) = ValidationHelper.ValidateModel(cd);
            if (!isValid)
                return this.BadRequestResult("Dữ liệu không hợp lệ", errors);

            var existing = await _context.ChuyenDeNCKHs.FindAsync(id);
            if (existing == null)
                return this.NotFoundResult("Không tìm thấy chuyên đề");

            if (await _context.ChuyenDeNCKHs.AnyAsync(x => x.MaSoCD == cd.MaSoCD && x.Id != id))
                return this.BadRequestResult("Mã chuyên đề đã tồn tại!");

            cd.MaSoCD = cd.MaSoCD.Trim().ToUpper();
            cd.TenChuyenDe = cd.TenChuyenDe.Trim();
            _context.Entry(existing).CurrentValues.SetValues(cd);

            await _context.SaveChangesAsync();
            return this.OkResult("Cập nhật chuyên đề thành công");
        }

        // DELETE: api/ChuyenDeNCKH/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            if (id <= 0)
                return this.BadRequestResult("ID không hợp lệ");

            var cd = await _context.ChuyenDeNCKHs.FindAsync(id);
            if (cd == null)
                return this.NotFoundResult("Không tìm thấy chuyên đề");

            // Xóa các dữ liệu liên quan theo thứ tự
            await _context.XepGiais.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();
            await _context.PhieuChams.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();
            await _context.KetQuaSoLoais.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();
            await _context.NopSanPhams.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();

            var hoiDongIds = await _context.HoiDongs.Where(x => x.IdChuyenDe == id).Select(x => x.Id).ToListAsync();
            await _context.ThanhVienHoiDongs.Where(x => hoiDongIds.Contains(x.IdHoiDong)).ExecuteDeleteAsync();
            await _context.HoiDongs.Where(x => x.IdChuyenDe == id).ExecuteDeleteAsync();

            _context.ChuyenDeNCKHs.Remove(cd);
            await _context.SaveChangesAsync();

            return this.OkResult("Xóa chuyên đề thành công");
        }
    }
}
