using System.Net.Http.Json;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Client.Extensions;
using QLNCKH_HocVien.Client.Exceptions;

namespace QLNCKH_HocVien.Client.Services
{
    public class KetQuaService
    {
        private readonly HttpClient _http;

        public KetQuaService(HttpClient http)
        {
            _http = http;
        }

        // ==================== VÒNG SƠ LOẠI ====================

        /// <summary>
        /// Lấy tất cả kết quả sơ loại
        /// </summary>
        public async Task<List<KetQuaSoLoai>> GetSoLoai()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<KetQuaSoLoai>>>("api/KetQua/soloai-all");
            return result?.Data ?? new List<KetQuaSoLoai>();
        }

        /// <summary>
        /// Lấy kết quả sơ loại có phân trang
        /// </summary>
        public async Task<PaginatedResult<KetQuaSoLoai>> GetSoLoaiPaged(
            int pageNumber = 1,
            int pageSize = 10,
            bool? ketQua = null)
        {
            var url = $"api/KetQua/soloai/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (ketQua.HasValue)
                url += $"&ketQua={ketQua}";

            var result = await _http.GetFromJsonAsyncWithAuth<PaginatedResult<KetQuaSoLoai>>(url);
            return result ?? new PaginatedResult<KetQuaSoLoai>();
        }

        /// <summary>
        /// Lấy kết quả sơ loại theo chuyên đề
        /// </summary>
        public async Task<KetQuaSoLoai?> GetSoLoaiByChuyenDe(int idChuyenDe)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<KetQuaSoLoai>>($"api/KetQua/soloai/{idChuyenDe}");
            return result?.Data;
        }

        /// <summary>
        /// Lưu kết quả sơ loại
        /// </summary>
        public async Task<ApiResult> SaveSoLoai(KetQuaSoLoai item)
        {
            var response = await _http.PostAsJsonAsyncWithAuth("api/KetQua/soloai", item);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        /// <summary>
        /// Tự động xét Top 15
        /// </summary>
        public async Task<ApiResult<int>> AutoTop15()
        {
            var response = await _http.PostAsyncWithAuth("api/KetQua/auto-top15", null);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult<int>.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult<int>.Fail("Không có response từ server");
        }

        // ==================== VÒNG CHUNG KHẢO ====================

        /// <summary>
        /// Lấy tất cả phiếu chấm
        /// </summary>
        public async Task<List<PhieuCham>> GetAllPhieuCham()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<PhieuCham>>>("api/KetQua/phieucham-all");
            return result?.Data ?? new List<PhieuCham>();
        }

        /// <summary>
        /// Lấy phiếu chấm có phân trang
        /// </summary>
        public async Task<PaginatedResult<PhieuCham>> GetPhieuChamPaged(
            int pageNumber = 1,
            int pageSize = 10,
            int? idChuyenDe = null,
            int? idGiaoVien = null)
        {
            var url = $"api/KetQua/phieucham/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (idChuyenDe.HasValue)
                url += $"&idChuyenDe={idChuyenDe}";
            if (idGiaoVien.HasValue)
                url += $"&idGiaoVien={idGiaoVien}";

            var result = await _http.GetFromJsonAsyncWithAuth<PaginatedResult<PhieuCham>>(url);
            return result ?? new PaginatedResult<PhieuCham>();
        }

        /// <summary>
        /// Lấy phiếu chấm theo chuyên đề
        /// </summary>
        public async Task<List<PhieuCham>> GetPhieuCham(int idChuyenDe)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<PhieuCham>>>($"api/KetQua/phieucham/{idChuyenDe}");
            return result?.Data ?? new List<PhieuCham>();
        }

        /// <summary>
        /// Lưu phiếu chấm
        /// </summary>
        public async Task<ApiResult> SavePhieuCham(PhieuCham item)
        {
            var response = await _http.PostAsJsonAsyncWithAuth("api/KetQua/phieucham", item);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        /// <summary>
        /// Xóa phiếu chấm
        /// </summary>
        public async Task<ApiResult> DeletePhieuCham(int id)
        {
            var response = await _http.DeleteAsyncWithAuth($"api/KetQua/phieucham/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        // ==================== DỮ LIỆU THAM CHIẾU ====================

        public async Task<List<ChuyenDeNCKH>> GetChuyenDe()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<ChuyenDeNCKH>>>("api/ChuyenDeNCKH");
            return result?.Data ?? new List<ChuyenDeNCKH>();
        }

        public async Task<List<HoiDong>> GetHoiDong()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<HoiDong>>>("api/HoiDong");
            return result?.Data ?? new List<HoiDong>();
        }

        public async Task<List<GiaoVien>> GetGiaoVien()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<GiaoVien>>>("api/GiaoVien");
            return result?.Data ?? new List<GiaoVien>();
        }
    }
}
