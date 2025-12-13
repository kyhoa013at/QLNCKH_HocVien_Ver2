using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Client.Extensions;
using QLNCKH_HocVien.Client.Exceptions;
using System.Net.Http.Json;

namespace QLNCKH_HocVien.Client.Services
{
    public class SinhVienService
    {
        private readonly HttpClient _http;

        public SinhVienService(HttpClient http)
        {
            _http = http;
        }

        // --- PHẦN 1: GỌI API DANH MỤC (API ngoài) ---
        public async Task<List<Tinh>> LayDsTinh()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<Tinh>>>("http://apidanhmuc.6pg.org/api/tinh/getall");
            return result?.Data ?? new List<Tinh>();
        }

        public async Task<List<DanToc>> LayDsDanToc()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<DanToc>>>("http://apidanhmuc.6pg.org/api/dantoc/getall");
            return result?.Data ?? new List<DanToc>();
        }

        public async Task<List<TonGiao>> LayDsTonGiao()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<TonGiao>>>("http://apidanhmuc.6pg.org/api/tongiao/getall");
            return result?.Data ?? new List<TonGiao>();
        }

        public async Task<List<ChucVu>> LayDsChucVu()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<ChucVu>>>("http://apidanhmuc.6pg.org/api/chucvu/getall");
            return result?.Data ?? new List<ChucVu>();
        }

        public async Task<List<Nganh>> LayDsNganh()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<Nganh>>>("http://apidanhmuc.6pg.org/api/lvnganh/getall");
            return result?.Data ?? new List<Nganh>();
        }

        // --- PHẦN 2: QUẢN LÝ SINH VIÊN (API nội bộ) ---

        /// <summary>
        /// Lấy tất cả sinh viên (backward compatible)
        /// </summary>
        public async Task<List<SinhVien>> LayTatCaSinhVien()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<SinhVien>>>("api/SinhVien");
            return result?.Data ?? new List<SinhVien>();
        }

        /// <summary>
        /// Lấy sinh viên có phân trang
        /// </summary>
        public async Task<PaginatedResult<SinhVien>> LayDanhSachPhanTrang(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var url = $"api/SinhVien/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={Uri.EscapeDataString(search)}";

            var result = await _http.GetFromJsonAsyncWithAuth<PaginatedResult<SinhVien>>(url);
            return result ?? new PaginatedResult<SinhVien>();
        }

        /// <summary>
        /// Lấy sinh viên theo ID
        /// </summary>
        public async Task<SinhVien?> LayTheoId(int id)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<SinhVien>>($"api/SinhVien/{id}");
            return result?.Data;
        }

        /// <summary>
        /// Thêm sinh viên mới
        /// </summary>
        public async Task<ApiResult<SinhVien>> ThemSinhVien(SinhVien sv)
        {
            var response = await _http.PostAsJsonAsyncWithAuth("api/SinhVien", sv);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult<SinhVien>>();
            
            if (!response.IsSuccessStatusCode && result == null)
            {
                return ApiResult<SinhVien>.Fail($"Lỗi: {response.StatusCode}");
            }

            return result ?? ApiResult<SinhVien>.Fail("Không có response từ server");
        }

        /// <summary>
        /// Cập nhật sinh viên
        /// </summary>
        public async Task<ApiResult> CapNhatSinhVien(SinhVien sv)
        {
            var response = await _http.PutAsJsonAsync($"api/SinhVien/{sv.Id}", sv);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
            {
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");
            }

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        /// <summary>
        /// Xóa sinh viên
        /// </summary>
        public async Task<ApiResult> XoaSinhVien(int id)
        {
            var response = await _http.DeleteAsyncWithAuth($"api/SinhVien/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
            {
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");
            }

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        /// <summary>
        /// URL xuất Excel
        /// </summary>
        public string GetExportUrl() => "api/SinhVien/export";
    }
}
