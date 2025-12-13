using System.Net.Http.Json;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Client.Extensions;
using QLNCKH_HocVien.Client.Exceptions;

namespace QLNCKH_HocVien.Client.Services
{
    public class XepGiaiService
    {
        private readonly HttpClient _http;

        public XepGiaiService(HttpClient http)
        {
            _http = http;
        }

        // ==================== API XẾP GIẢI ====================

        /// <summary>
        /// Lấy tất cả kết quả xếp giải
        /// </summary>
        public async Task<List<XepGiai>> LayKetQua()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<XepGiai>>>("api/XepGiai");
            return result?.Data ?? new List<XepGiai>();
        }

        /// <summary>
        /// Lấy kết quả xếp giải có phân trang
        /// </summary>
        public async Task<PaginatedResult<XepGiai>> LayKetQuaPhanTrang(
            int pageNumber = 1,
            int pageSize = 10,
            string? tenGiai = null)
        {
            var url = $"api/XepGiai/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(tenGiai))
                url += $"&tenGiai={Uri.EscapeDataString(tenGiai)}";

            var result = await _http.GetFromJsonAsyncWithAuth<PaginatedResult<XepGiai>>(url);
            return result ?? new PaginatedResult<XepGiai>();
        }

        /// <summary>
        /// Lấy kết quả xếp giải theo chuyên đề
        /// </summary>
        public async Task<XepGiai?> LayTheoChuyenDe(int idChuyenDe)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<XepGiai>>($"api/XepGiai/by-chuyende/{idChuyenDe}");
            return result?.Data;
        }

        /// <summary>
        /// Lấy thống kê xếp giải
        /// </summary>
        public async Task<object?> LaySummary()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<object>>("api/XepGiai/summary");
            return result?.Data;
        }

        /// <summary>
        /// Tự động xếp giải
        /// </summary>
        public async Task<ApiResult<List<XepGiai>>> TuDongXepGiai()
        {
            var response = await _http.PostAsyncWithAuth("api/XepGiai/process", null);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult<List<XepGiai>>>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult<List<XepGiai>>.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult<List<XepGiai>>.Fail("Không có response từ server");
        }

        /// <summary>
        /// Reset kết quả xếp giải
        /// </summary>
        public async Task<ApiResult> ResetKetQua()
        {
            var response = await _http.DeleteAsyncWithAuth("api/XepGiai/reset");
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        // ==================== DỮ LIỆU THAM CHIẾU ====================

        public async Task<List<ChuyenDeNCKH>> LayDsChuyenDe()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<ChuyenDeNCKH>>>("api/ChuyenDeNCKH");
            return result?.Data ?? new List<ChuyenDeNCKH>();
        }

        public async Task<List<SinhVien>> LayDsSinhVien()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<SinhVien>>>("api/SinhVien");
            return result?.Data ?? new List<SinhVien>();
        }

        public async Task<List<Nganh>> LayDsNganh()
        {
            var res = await _http.GetFromJsonAsync<ApiResponse<List<Nganh>>>("http://apidanhmuc.6pg.org/api/lvnganh/getall");
            return res?.Data ?? new List<Nganh>();
        }
    }
}
