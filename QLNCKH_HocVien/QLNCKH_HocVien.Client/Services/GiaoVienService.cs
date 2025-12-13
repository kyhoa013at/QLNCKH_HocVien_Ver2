using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Client.Extensions;
using QLNCKH_HocVien.Client.Exceptions;
using System.Net.Http.Json;

namespace QLNCKH_HocVien.Client.Services
{
    public class GiaoVienService
    {
        private readonly HttpClient _http;

        public GiaoVienService(HttpClient http)
        {
            _http = http;
        }

        // --- API DANH MỤC NGOÀI ---
        public async Task<List<TrinhDoChuyenMon>> LayDsTrinhDoChuyenMon()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<TrinhDoChuyenMon>>>("http://apidanhmuc.6pg.org/api/tdcm/getall");
            return result?.Data ?? new List<TrinhDoChuyenMon>();
        }

        public async Task<List<TrinhDoLLCT>> LayDsTrinhDoLLCT()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<TrinhDoLLCT>>>("http://apidanhmuc.6pg.org/api/tdllct/getall");
            return result?.Data ?? new List<TrinhDoLLCT>();
        }

        public async Task<List<CapBac>> LayDsCapBac()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<CapBac>>>("http://apidanhmuc.6pg.org/api/capbac/getall");
            return result?.Data ?? new List<CapBac>();
        }

        public async Task<List<ToChuc>> LayDsToChuc()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<ToChuc>>>("http://apidanhmuc.6pg.org/api/tochuc/getall");
            return result?.Data ?? new List<ToChuc>();
        }

        public async Task<List<HocHam>> LayDsHocHam()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<HocHam>>>("http://apidanhmuc.6pg.org/api/hocham/getall");
            return result?.Data ?? new List<HocHam>();
        }

        public async Task<List<ChucDanh>> LayDsChucDanh()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<ChucDanh>>>("http://apidanhmuc.6pg.org/api/chucdanh/getall");
            return result?.Data ?? new List<ChucDanh>();
        }

        public async Task<List<HocVi>> LayDsHocVi()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<HocVi>>>("http://apidanhmuc.6pg.org/api/trinhdodaotao/getall");
            return result?.Data ?? new List<HocVi>();
        }

        public async Task<List<ChucVu>> LayDsChucVu()
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<ChucVu>>>("http://apidanhmuc.6pg.org/api/chucvu/getall");
            return result?.Data ?? new List<ChucVu>();
        }

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

        // --- API GIÁO VIÊN NỘI BỘ ---

        /// <summary>
        /// Lấy tất cả giáo viên
        /// </summary>
        public async Task<List<GiaoVien>> LayTatCaGiaoVien()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<GiaoVien>>>("api/GiaoVien");
            return result?.Data ?? new List<GiaoVien>();
        }

        /// <summary>
        /// Lấy giáo viên có phân trang
        /// </summary>
        public async Task<PaginatedResult<GiaoVien>> LayDanhSachPhanTrang(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var url = $"api/GiaoVien/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={Uri.EscapeDataString(search)}";

            var result = await _http.GetFromJsonAsyncWithAuth<PaginatedResult<GiaoVien>>(url);
            return result ?? new PaginatedResult<GiaoVien>();
        }

        /// <summary>
        /// Lấy giáo viên theo ID
        /// </summary>
        public async Task<GiaoVien?> LayTheoId(int id)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<GiaoVien>>($"api/GiaoVien/{id}");
            return result?.Data;
        }

        /// <summary>
        /// Thêm giáo viên mới
        /// </summary>
        public async Task<ApiResult<GiaoVien>> ThemGiaoVien(GiaoVien gv)
        {
            var response = await _http.PostAsJsonAsyncWithAuth("api/GiaoVien", gv);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult<GiaoVien>>();
            
            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult<GiaoVien>.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult<GiaoVien>.Fail("Không có response từ server");
        }

        /// <summary>
        /// Cập nhật giáo viên
        /// </summary>
        public async Task<ApiResult> CapNhatGiaoVien(GiaoVien gv)
        {
            var response = await _http.PutAsJsonAsync($"api/GiaoVien/{gv.Id}", gv);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        /// <summary>
        /// Xóa giáo viên
        /// </summary>
        public async Task<ApiResult> XoaGiaoVien(int id)
        {
            var response = await _http.DeleteAsyncWithAuth($"api/GiaoVien/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }
    }
}
