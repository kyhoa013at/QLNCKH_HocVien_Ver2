using Microsoft.Extensions.Logging;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Helpers;
using QLNCKH_HocVien.Repositories;

namespace QLNCKH_HocVien.Services
{
    // ==================== SINH VIÊN SERVICE ====================
    public class SinhVienBusinessService : ISinhVienService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SinhVienBusinessService> _logger;

        public SinhVienBusinessService(IUnitOfWork unitOfWork, ILogger<SinhVienBusinessService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResult<List<SinhVien>>> GetAllAsync()
        {
            _logger.LogInformation("Lấy tất cả sinh viên");
            var data = (await _unitOfWork.SinhViens.GetAllAsync()).ToList();
            return ApiResult<List<SinhVien>>.Ok(data, $"Lấy {data.Count} sinh viên");
        }

        public async Task<PaginatedResult<SinhVien>> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
        {
            _logger.LogInformation("Lấy sinh viên phân trang: Page={PageNumber}, Size={PageSize}, Search={Search}", 
                pageNumber, pageSize, search);
            
            var (items, totalCount) = await _unitOfWork.SinhViens.SearchAsync(pageNumber, pageSize, search);
            return PaginatedResult<SinhVien>.Create(items.ToList(), totalCount, pageNumber, pageSize);
        }

        public async Task<ApiResult<SinhVien>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResult<SinhVien>.Fail("ID không hợp lệ");

            var sv = await _unitOfWork.SinhViens.GetByIdAsync(id);
            if (sv == null)
                return ApiResult<SinhVien>.Fail("Không tìm thấy sinh viên");

            return ApiResult<SinhVien>.Ok(sv);
        }

        public async Task<ApiResult<SinhVien>> CreateAsync(SinhVien sv)
        {
            var (isValid, errors) = ValidationHelper.ValidateModel(sv);
            if (!isValid)
                return ApiResult<SinhVien>.Fail("Dữ liệu không hợp lệ", errors);

            if (await _unitOfWork.SinhViens.IsMaSVExistsAsync(sv.MaSV))
                return ApiResult<SinhVien>.Fail("Mã sinh viên đã tồn tại!");

            sv.MaSV = sv.MaSV.Trim().ToUpper();
            sv.HoTen = sv.HoTen.Trim();

            await _unitOfWork.SinhViens.AddAsync(sv);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Tạo sinh viên mới: {MaSV} - {HoTen}", sv.MaSV, sv.HoTen);
            return ApiResult<SinhVien>.Ok(sv, "Thêm sinh viên thành công");
        }

        public async Task<ApiResult> UpdateAsync(int id, SinhVien sv)
        {
            if (id <= 0 || id != sv.Id)
                return ApiResult.Fail("ID không hợp lệ");

            var (isValid, errors) = ValidationHelper.ValidateModel(sv);
            if (!isValid)
                return ApiResult.Fail("Dữ liệu không hợp lệ", errors);

            var existing = await _unitOfWork.SinhViens.GetByIdAsync(id);
            if (existing == null)
                return ApiResult.Fail("Không tìm thấy sinh viên");

            if (await _unitOfWork.SinhViens.IsMaSVExistsAsync(sv.MaSV, id))
                return ApiResult.Fail("Mã sinh viên đã tồn tại!");

            existing.MaSV = sv.MaSV.Trim().ToUpper();
            existing.HoTen = sv.HoTen.Trim();
            existing.GioiTinh = sv.GioiTinh;
            existing.NgaySinh = sv.NgaySinh;
            existing.IdTinh = sv.IdTinh;
            existing.IdXa = sv.IdXa;
            existing.IdDanToc = sv.IdDanToc;
            existing.IdTonGiao = sv.IdTonGiao;
            existing.SoDienThoai = sv.SoDienThoai;
            existing.Lop = sv.Lop;
            existing.IdChucVu = sv.IdChucVu;
            existing.IdNganh = sv.IdNganh;
            existing.ChuyenNganh = sv.ChuyenNganh;

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Cập nhật sinh viên: {Id} - {MaSV}", id, sv.MaSV);
            return ApiResult.Ok("Cập nhật sinh viên thành công");
        }

        public async Task<ApiResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return ApiResult.Fail("ID không hợp lệ");

            var sv = await _unitOfWork.SinhViens.GetByIdAsync(id);
            if (sv == null)
                return ApiResult.Fail("Không tìm thấy sinh viên");

            if (await _unitOfWork.ChuyenDes.AnyAsync(c => c.IdHocVien == id))
                return ApiResult.Fail("Không thể xóa sinh viên đang thực hiện chuyên đề!");

            _unitOfWork.SinhViens.Remove(sv);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Xóa sinh viên: {Id} - {MaSV}", id, sv.MaSV);
            return ApiResult.Ok("Xóa sinh viên thành công");
        }
    }

    // ==================== GIÁO VIÊN SERVICE ====================
    public class GiaoVienBusinessService : IGiaoVienService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GiaoVienBusinessService> _logger;

        public GiaoVienBusinessService(IUnitOfWork unitOfWork, ILogger<GiaoVienBusinessService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResult<List<GiaoVien>>> GetAllAsync()
        {
            var data = (await _unitOfWork.GiaoViens.GetAllAsync()).ToList();
            return ApiResult<List<GiaoVien>>.Ok(data, $"Lấy {data.Count} giáo viên");
        }

        public async Task<PaginatedResult<GiaoVien>> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
        {
            var (items, totalCount) = await _unitOfWork.GiaoViens.SearchAsync(pageNumber, pageSize, search);
            return PaginatedResult<GiaoVien>.Create(items.ToList(), totalCount, pageNumber, pageSize);
        }

        public async Task<ApiResult<GiaoVien>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResult<GiaoVien>.Fail("ID không hợp lệ");

            var gv = await _unitOfWork.GiaoViens.GetByIdAsync(id);
            if (gv == null)
                return ApiResult<GiaoVien>.Fail("Không tìm thấy giáo viên");

            return ApiResult<GiaoVien>.Ok(gv);
        }

        public async Task<ApiResult<GiaoVien>> CreateAsync(GiaoVien gv)
        {
            var (isValid, errors) = ValidationHelper.ValidateModel(gv);
            if (!isValid)
                return ApiResult<GiaoVien>.Fail("Dữ liệu không hợp lệ", errors);

            if (await _unitOfWork.GiaoViens.IsMaSoCBExistsAsync(gv.MaSoCB))
                return ApiResult<GiaoVien>.Fail("Mã số cán bộ đã tồn tại!");

            gv.MaSoCB = gv.MaSoCB.Trim().ToUpper();
            gv.HoTen = gv.HoTen.Trim();

            await _unitOfWork.GiaoViens.AddAsync(gv);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Tạo giáo viên mới: {MaSoCB} - {HoTen}", gv.MaSoCB, gv.HoTen);
            return ApiResult<GiaoVien>.Ok(gv, "Thêm giáo viên thành công");
        }

        public async Task<ApiResult> UpdateAsync(int id, GiaoVien gv)
        {
            if (id <= 0 || id != gv.Id)
                return ApiResult.Fail("ID không hợp lệ");

            var (isValid, errors) = ValidationHelper.ValidateModel(gv);
            if (!isValid)
                return ApiResult.Fail("Dữ liệu không hợp lệ", errors);

            var existing = await _unitOfWork.GiaoViens.GetByIdAsync(id);
            if (existing == null)
                return ApiResult.Fail("Không tìm thấy giáo viên");

            if (await _unitOfWork.GiaoViens.IsMaSoCBExistsAsync(gv.MaSoCB, id))
                return ApiResult.Fail("Mã số cán bộ đã tồn tại!");

            existing.MaSoCB = gv.MaSoCB.Trim().ToUpper();
            existing.HoTen = gv.HoTen.Trim();
            existing.GioiTinh = gv.GioiTinh;
            existing.NgaySinh = gv.NgaySinh;
            existing.IdTinh = gv.IdTinh;
            existing.IdXa = gv.IdXa;
            existing.IdDanToc = gv.IdDanToc;
            existing.IdTonGiao = gv.IdTonGiao;
            existing.SoDienThoai = gv.SoDienThoai;
            existing.IdTrinhDoChuyenMon = gv.IdTrinhDoChuyenMon;
            existing.IdTrinhDoLLCT = gv.IdTrinhDoLLCT;
            existing.IdDonViCongTac = gv.IdDonViCongTac;
            existing.IdChucVu = gv.IdChucVu;
            existing.IdCapBac = gv.IdCapBac;
            existing.HeSoLuong = gv.HeSoLuong;
            existing.IdChucDanh = gv.IdChucDanh;
            existing.IdHocHam = gv.IdHocHam;
            existing.IdHocVi = gv.IdHocVi;
            existing.LinhVuc = gv.LinhVuc;

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Cập nhật giáo viên: {Id} - {MaSoCB}", id, gv.MaSoCB);
            return ApiResult.Ok("Cập nhật giáo viên thành công");
        }

        public async Task<ApiResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return ApiResult.Fail("ID không hợp lệ");

            var gv = await _unitOfWork.GiaoViens.GetByIdAsync(id);
            if (gv == null)
                return ApiResult.Fail("Không tìm thấy giáo viên");

            if (await _unitOfWork.GiaoViens.IsInHoiDongAsync(id))
                return ApiResult.Fail("Không thể xóa giáo viên đang tham gia hội đồng chấm!");

            _unitOfWork.GiaoViens.Remove(gv);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Xóa giáo viên: {Id} - {MaSoCB}", id, gv.MaSoCB);
            return ApiResult.Ok("Xóa giáo viên thành công");
        }
    }

    // ==================== CHUYÊN ĐỀ SERVICE ====================
    public class ChuyenDeBusinessService : IChuyenDeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChuyenDeBusinessService> _logger;

        public ChuyenDeBusinessService(IUnitOfWork unitOfWork, ILogger<ChuyenDeBusinessService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResult<List<ChuyenDeNCKH>>> GetAllAsync()
        {
            var data = (await _unitOfWork.ChuyenDes.GetAllAsync()).ToList();
            return ApiResult<List<ChuyenDeNCKH>>.Ok(data, $"Lấy {data.Count} chuyên đề");
        }

        public async Task<PaginatedResult<ChuyenDeNCKH>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, int? idLinhVuc = null)
        {
            var (items, totalCount) = await _unitOfWork.ChuyenDes.SearchAsync(pageNumber, pageSize, search, idLinhVuc);
            return PaginatedResult<ChuyenDeNCKH>.Create(items.ToList(), totalCount, pageNumber, pageSize);
        }

        public async Task<ApiResult<ChuyenDeNCKH>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResult<ChuyenDeNCKH>.Fail("ID không hợp lệ");

            var cd = await _unitOfWork.ChuyenDes.GetByIdAsync(id);
            if (cd == null)
                return ApiResult<ChuyenDeNCKH>.Fail("Không tìm thấy chuyên đề");

            return ApiResult<ChuyenDeNCKH>.Ok(cd);
        }

        public async Task<ApiResult<ChuyenDeNCKH>> CreateAsync(ChuyenDeNCKH cd)
        {
            var (isValid, errors) = ValidationHelper.ValidateModel(cd);
            if (!isValid)
                return ApiResult<ChuyenDeNCKH>.Fail("Dữ liệu không hợp lệ", errors);

            if (!await _unitOfWork.SinhViens.AnyAsync(s => s.Id == cd.IdHocVien))
                return ApiResult<ChuyenDeNCKH>.Fail("Học viên không tồn tại!");

            if (await _unitOfWork.ChuyenDes.IsMaSoCDExistsAsync(cd.MaSoCD))
                return ApiResult<ChuyenDeNCKH>.Fail("Mã chuyên đề đã tồn tại!");

            cd.MaSoCD = cd.MaSoCD.Trim().ToUpper();
            cd.TenChuyenDe = cd.TenChuyenDe.Trim();

            await _unitOfWork.ChuyenDes.AddAsync(cd);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Tạo chuyên đề mới: {MaSoCD} - {TenChuyenDe}", cd.MaSoCD, cd.TenChuyenDe);
            return ApiResult<ChuyenDeNCKH>.Ok(cd, "Đăng ký chuyên đề thành công");
        }

        public async Task<ApiResult> UpdateAsync(int id, ChuyenDeNCKH cd)
        {
            if (id <= 0 || id != cd.Id)
                return ApiResult.Fail("ID không hợp lệ");

            var (isValid, errors) = ValidationHelper.ValidateModel(cd);
            if (!isValid)
                return ApiResult.Fail("Dữ liệu không hợp lệ", errors);

            var existing = await _unitOfWork.ChuyenDes.GetByIdAsync(id);
            if (existing == null)
                return ApiResult.Fail("Không tìm thấy chuyên đề");

            if (await _unitOfWork.ChuyenDes.IsMaSoCDExistsAsync(cd.MaSoCD, id))
                return ApiResult.Fail("Mã chuyên đề đã tồn tại!");

            existing.MaSoCD = cd.MaSoCD.Trim().ToUpper();
            existing.TenChuyenDe = cd.TenChuyenDe.Trim();
            existing.IdHocVien = cd.IdHocVien;
            existing.IdLinhVuc = cd.IdLinhVuc;

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Cập nhật chuyên đề: {Id} - {MaSoCD}", id, cd.MaSoCD);
            return ApiResult.Ok("Cập nhật chuyên đề thành công");
        }

        public async Task<ApiResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return ApiResult.Fail("ID không hợp lệ");

            var cd = await _unitOfWork.ChuyenDes.GetByIdAsync(id);
            if (cd == null)
                return ApiResult.Fail("Không tìm thấy chuyên đề");

            await _unitOfWork.ChuyenDes.DeleteWithRelatedDataAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Xóa chuyên đề: {Id} - {MaSoCD}", id, cd.MaSoCD);
            return ApiResult.Ok("Xóa chuyên đề thành công");
        }
    }
}

