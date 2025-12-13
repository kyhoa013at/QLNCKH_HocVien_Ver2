using System.Net.Http.Json;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Client.Extensions;
using QLNCKH_HocVien.Client.Exceptions;

namespace QLNCKH_HocVien.Client.Services
{
    public class HoiDongService
    {
        private readonly HttpClient _http;

        public HoiDongService(HttpClient http)
        {
            _http = http;
        }

        // ==================== API HỘI ĐỒNG ====================

        /// <summary>
        /// Lấy tất cả hội đồng
        /// </summary>
        public async Task<List<HoiDong>> LayDsHoiDong()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<HoiDong>>>("api/HoiDong");
            return result?.Data ?? new List<HoiDong>();
        }

        /// <summary>
        /// Lấy hội đồng có phân trang
        /// </summary>
        public async Task<PaginatedResult<HoiDong>> LayDanhSachPhanTrang(
            int pageNumber = 1,
            int pageSize = 10,
            VongCham? vongThi = null)
        {
            var url = $"api/HoiDong/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (vongThi.HasValue)
                url += $"&vongThi={(int)vongThi}";

            var result = await _http.GetFromJsonAsyncWithAuth<PaginatedResult<HoiDong>>(url);
            return result ?? new PaginatedResult<HoiDong>();
        }

        /// <summary>
        /// Lấy hội đồng theo chuyên đề
        /// </summary>
        public async Task<List<HoiDong>> LayTheoChuyenDe(int idChuyenDe)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<HoiDong>>>($"api/HoiDong/by-chuyende/{idChuyenDe}");
            return result?.Data ?? new List<HoiDong>();
        }

        /// <summary>
        /// Lấy hội đồng theo ID
        /// </summary>
        public async Task<HoiDong?> LayTheoId(int id)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<HoiDong>>($"api/HoiDong/{id}");
            return result?.Data;
        }

        /// <summary>
        /// Tạo hội đồng mới
        /// </summary>
        public async Task<ApiResult<HoiDong>> TaoHoiDong(HoiDong hd)
        {
            var response = await _http.PostAsJsonAsyncWithAuth("api/HoiDong", hd);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult<HoiDong>>();
            
            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult<HoiDong>.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult<HoiDong>.Fail("Không có response từ server");
        }

        /// <summary>
        /// Cập nhật hội đồng
        /// </summary>
        public async Task<ApiResult> CapNhatHoiDong(HoiDong hd)
        {
            var response = await _http.PutAsJsonAsync($"api/HoiDong/{hd.Id}", hd);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        /// <summary>
        /// Xóa hội đồng
        /// </summary>
        public async Task<ApiResult> XoaHoiDong(int id)
        {
            var response = await _http.DeleteAsyncWithAuth($"api/HoiDong/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        // ==================== DỮ LIỆU THAM CHIẾU ====================

        /// <summary>
        /// Lấy danh sách chuyên đề
        /// </summary>
        public async Task<List<ChuyenDeNCKH>> LayDsChuyenDe()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<ChuyenDeNCKH>>>("api/ChuyenDeNCKH");
            return result?.Data ?? new List<ChuyenDeNCKH>();
        }

        /// <summary>
        /// Lấy danh sách giáo viên
        /// </summary>
        public async Task<List<GiaoVien>> LayDsGiaoVien()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<GiaoVien>>>("api/GiaoVien");
            return result?.Data ?? new List<GiaoVien>();
        }

        /// <summary>
        /// Lấy danh sách tổ chức (API ngoài)
        /// </summary>
        public async Task<List<ToChuc>> LayDsToChuc()
        {
            var res = await _http.GetFromJsonAsync<ApiResponse<List<ToChuc>>>("http://apidanhmuc.6pg.org/api/tochuc/getall");
            return res?.Data ?? new List<ToChuc>();
        }

        /// <summary>
        /// Lấy danh sách chức vụ (API ngoài)
        /// </summary>
        public async Task<List<ChucVu>> LayDsChucVu()
        {
            var res = await _http.GetFromJsonAsync<ApiResponse<List<ChucVu>>>("http://apidanhmuc.6pg.org/api/chucvu/getall");
            return res?.Data ?? new List<ChucVu>();
        }
    }
}
