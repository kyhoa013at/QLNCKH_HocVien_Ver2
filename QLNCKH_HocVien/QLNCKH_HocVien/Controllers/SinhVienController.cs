using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Data;
using QLNCKH_HocVien.Helpers;
using ClosedXML.Excel;

namespace QLNCKH_HocVien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SinhVienController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SinhVien (Lấy tất cả - backward compatible)
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<SinhVien>>>> GetSinhViens()
        {
            var data = await _context.SinhViens.ToListAsync();
            return this.OkResult(data, $"Lấy {data.Count} sinh viên");
        }

        // GET: api/SinhVien/paged?pageNumber=1&pageSize=10&search=abc
        [HttpGet("paged")]
        public async Task<ActionResult<PaginatedResult<SinhVien>>> GetSinhViensPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            // Validate pagination params
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.SinhViens.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(s => 
                    s.MaSV.ToLower().Contains(search) || 
                    s.HoTen.ToLower().Contains(search) ||
                    (s.Lop != null && s.Lop.ToLower().Contains(search)));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(s => s.MaSV)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return this.PaginatedOk(items, totalCount, pageNumber, pageSize);
        }

        // GET: api/SinhVien/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<SinhVien>>> GetSinhVien(int id)
        {
            if (id <= 0) 
                return this.BadRequestResult<SinhVien>("ID không hợp lệ");

            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) 
                return this.NotFoundResult<SinhVien>("Không tìm thấy sinh viên");

            return this.OkResult(sv);
        }

        // POST: api/SinhVien (Thêm mới)
        [HttpPost]
        public async Task<ActionResult<ApiResult<SinhVien>>> CreateSinhVien(SinhVien sv)
        {
            // Server-side validation
            var (isValid, errors) = ValidationHelper.ValidateModel(sv);
            if (!isValid)
                return this.BadRequestResult<SinhVien>("Dữ liệu không hợp lệ", errors);

            // Kiểm tra trùng mã sinh viên
            if (await _context.SinhViens.AnyAsync(x => x.MaSV == sv.MaSV))
                return this.BadRequestResult<SinhVien>("Mã sinh viên đã tồn tại!");

            // Sanitize input
            sv.MaSV = sv.MaSV.Trim().ToUpper();
            sv.HoTen = sv.HoTen.Trim();

            _context.SinhViens.Add(sv);
            await _context.SaveChangesAsync();
            return this.CreatedResult(sv, "Thêm sinh viên thành công");
        }

        // PUT: api/SinhVien/5 (Cập nhật)
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> UpdateSinhVien(int id, SinhVien sv)
        {
            if (id <= 0) 
                return this.BadRequestResult("ID không hợp lệ");
            if (id != sv.Id) 
                return this.BadRequestResult("ID không khớp");

            var (isValid, errors) = ValidationHelper.ValidateModel(sv);
            if (!isValid)
                return this.BadRequestResult("Dữ liệu không hợp lệ", errors);

            var existingSv = await _context.SinhViens.FindAsync(id);
            if (existingSv == null) 
                return this.NotFoundResult("Không tìm thấy sinh viên");

            // Kiểm tra trùng mã (trừ chính nó)
            if (await _context.SinhViens.AnyAsync(x => x.MaSV == sv.MaSV && x.Id != id))
                return this.BadRequestResult("Mã sinh viên đã tồn tại!");

            // Sanitize và cập nhật
            sv.MaSV = sv.MaSV.Trim().ToUpper();
            sv.HoTen = sv.HoTen.Trim();
            _context.Entry(existingSv).CurrentValues.SetValues(sv);

            await _context.SaveChangesAsync();
            return this.OkResult("Cập nhật sinh viên thành công");
        }

        // DELETE: api/SinhVien/5 (Xóa theo ID)
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> DeleteSinhVien(int id)
        {
            if (id <= 0) 
                return this.BadRequestResult("ID không hợp lệ");

            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) 
                return this.NotFoundResult("Không tìm thấy sinh viên");

            // Kiểm tra sinh viên có đang thực hiện chuyên đề không
            var hasChuyenDe = await _context.ChuyenDeNCKHs.AnyAsync(x => x.IdHocVien == id);
            if (hasChuyenDe)
                return this.BadRequestResult("Không thể xóa sinh viên đang thực hiện chuyên đề!");

            _context.SinhViens.Remove(sv);
            await _context.SaveChangesAsync();
            return this.OkResult("Xóa sinh viên thành công");
        }

        // GET: api/SinhVien/export (Xuất Excel)
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel()
        {
            var sinhViens = await _context.SinhViens.ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách sinh viên");

            // Header
            worksheet.Cell(1, 1).Value = "STT";
            worksheet.Cell(1, 2).Value = "Mã SV";
            worksheet.Cell(1, 3).Value = "Họ và Tên";
            worksheet.Cell(1, 4).Value = "Ngày sinh";
            worksheet.Cell(1, 5).Value = "Giới tính";
            worksheet.Cell(1, 6).Value = "Lớp";
            worksheet.Cell(1, 7).Value = "SĐT";

            // Định dạng header
            var headerRow = worksheet.Range("A1:G1");
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data
            int row = 2;
            int stt = 1;
            foreach (var sv in sinhViens)
            {
                worksheet.Cell(row, 1).Value = stt++;
                worksheet.Cell(row, 2).Value = sv.MaSV;
                worksheet.Cell(row, 3).Value = sv.HoTen;
                worksheet.Cell(row, 4).Value = sv.NgaySinh?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cell(row, 5).Value = sv.GioiTinh;
                worksheet.Cell(row, 6).Value = sv.Lop ?? "";
                worksheet.Cell(row, 7).Value = sv.SoDienThoai ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"DanhSachSinhVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
