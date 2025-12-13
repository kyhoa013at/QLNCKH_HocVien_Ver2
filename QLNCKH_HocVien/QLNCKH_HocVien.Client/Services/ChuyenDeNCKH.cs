using System.Net.Http.Json;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Client.Extensions;
using QLNCKH_HocVien.Client.Exceptions;

namespace QLNCKH_HocVien.Client.Services
{
    public class ChuyenDeNCKHService
    {
        private readonly HttpClient _http;

        public ChuyenDeNCKHService(HttpClient http)
        {
            _http = http;
        }

        // ==================== CRUD CHUYÊN ĐỀ ====================

        /// <summary>
        /// Lấy tất cả chuyên đề
        /// </summary>
        public async Task<List<ChuyenDeNCKH>> LayDsChuyenDe()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<ChuyenDeNCKH>>>("api/ChuyenDeNCKH");
            return result?.Data ?? new List<ChuyenDeNCKH>();
        }

        /// <summary>
        /// Lấy chuyên đề có phân trang
        /// </summary>
        public async Task<PaginatedResult<ChuyenDeNCKH>> LayDanhSachPhanTrang(
            int pageNumber = 1, 
            int pageSize = 10, 
            string? search = null,
            int? idLinhVuc = null)
        {
            var url = $"api/ChuyenDeNCKH/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={Uri.EscapeDataString(search)}";
            if (idLinhVuc.HasValue)
                url += $"&idLinhVuc={idLinhVuc}";

            var result = await _http.GetFromJsonAsyncWithAuth<PaginatedResult<ChuyenDeNCKH>>(url);
            return result ?? new PaginatedResult<ChuyenDeNCKH>();
        }

        /// <summary>
        /// Lấy chuyên đề theo ID
        /// </summary>
        public async Task<ChuyenDeNCKH?> LayTheoId(int id)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<ChuyenDeNCKH>>($"api/ChuyenDeNCKH/{id}");
            return result?.Data;
        }

        /// <summary>
        /// Thêm chuyên đề mới
        /// </summary>
        public async Task<ApiResult<ChuyenDeNCKH>> ThemChuyenDe(ChuyenDeNCKH cd)
        {
            var response = await _http.PostAsJsonAsyncWithAuth("api/ChuyenDeNCKH", cd);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult<ChuyenDeNCKH>>();
            
            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult<ChuyenDeNCKH>.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult<ChuyenDeNCKH>.Fail("Không có response từ server");
        }

        /// <summary>
        /// Cập nhật chuyên đề
        /// </summary>
        public async Task<ApiResult> CapNhatChuyenDe(ChuyenDeNCKH cd)
        {
            var response = await _http.PutAsJsonAsync($"api/ChuyenDeNCKH/{cd.Id}", cd);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        /// <summary>
        /// Xóa chuyên đề
        /// </summary>
        public async Task<ApiResult> XoaChuyenDe(int id)
        {
            var response = await _http.DeleteAsyncWithAuth($"api/ChuyenDeNCKH/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        // ==================== DỮ LIỆU THAM CHIẾU ====================

        /// <summary>
        /// Lấy danh sách sinh viên (để chọn người làm chuyên đề)
        /// </summary>
        public async Task<List<SinhVien>> LayDsSinhVien()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<SinhVien>>>("api/SinhVien");
            return result?.Data ?? new List<SinhVien>();
        }

        /// <summary>
        /// Lấy danh sách lĩnh vực (API ngoài)
        /// </summary>
        public async Task<List<LinhVucDeTai>> LayDsLinhVuc()
        {
            var res = await _http.GetFromJsonAsync<ApiResponse<List<LinhVucDeTai>>>("http://apidanhmuc.6pg.org/api/linhvucdetai/getall");
            return res?.Data ?? new List<LinhVucDeTai>();
        }

        /// <summary>
        /// Lấy danh sách ngành (API ngoài - để hiển thị thông tin sinh viên)
        /// </summary>
        public async Task<List<Nganh>> LayDsNganh()
        {
            var res = await _http.GetFromJsonAsync<ApiResponse<List<Nganh>>>("http://apidanhmuc.6pg.org/api/lvnganh/getall");
            return res?.Data ?? new List<Nganh>();
        }
    }
}
