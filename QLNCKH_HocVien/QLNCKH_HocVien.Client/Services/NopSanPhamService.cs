using System.Net.Http.Json;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Client.Extensions;
using QLNCKH_HocVien.Client.Exceptions;

namespace QLNCKH_HocVien.Client.Services
{
    public class NopSanPhamService
    {
        private readonly HttpClient _http;

        public NopSanPhamService(HttpClient http)
        {
            _http = http;
        }

        // ==================== API NỘP SẢN PHẨM ====================

        /// <summary>
        /// Lấy tất cả bản nộp
        /// </summary>
        public async Task<List<NopSanPham>> LayDsNop()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<NopSanPham>>>("api/NopSanPham");
            return result?.Data ?? new List<NopSanPham>();
        }

        /// <summary>
        /// Lấy bản nộp có phân trang
        /// </summary>
        public async Task<PaginatedResult<NopSanPham>> LayDanhSachPhanTrang(
            int pageNumber = 1,
            int pageSize = 10,
            int? idChuyenDe = null,
            TrangThaiNop? trangThai = null)
        {
            var url = $"api/NopSanPham/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (idChuyenDe.HasValue)
                url += $"&idChuyenDe={idChuyenDe}";
            if (trangThai.HasValue)
                url += $"&trangThai={(int)trangThai}";

            var result = await _http.GetFromJsonAsyncWithAuth<PaginatedResult<NopSanPham>>(url);
            return result ?? new PaginatedResult<NopSanPham>();
        }

        /// <summary>
        /// Lấy bản nộp theo chuyên đề
        /// </summary>
        public async Task<List<NopSanPham>> LayTheoChuyenDe(int idChuyenDe)
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<NopSanPham>>>($"api/NopSanPham/by-chuyende/{idChuyenDe}");
            return result?.Data ?? new List<NopSanPham>();
        }

        /// <summary>
        /// Nộp sản phẩm
        /// </summary>
        public async Task<ApiResult<NopSanPham>> NopBai(NopSanPham item)
        {
            var response = await _http.PostAsJsonAsyncWithAuth("api/NopSanPham", item);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult<NopSanPham>>();
            
            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult<NopSanPham>.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult<NopSanPham>.Fail("Không có response từ server");
        }

        /// <summary>
        /// Xóa bản nộp
        /// </summary>
        public async Task<ApiResult> XoaNop(int id)
        {
            var response = await _http.DeleteAsyncWithAuth($"api/NopSanPham/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (!response.IsSuccessStatusCode && result == null)
                return ApiResult.Fail($"Lỗi: {response.StatusCode}");

            return result ?? ApiResult.Fail("Không có response từ server");
        }

        // ==================== DỮ LIỆU THAM CHIẾU ====================

        /// <summary>
        /// Lấy danh sách chuyên đề (để chọn nộp cho cái nào)
        /// </summary>
        public async Task<List<ChuyenDeNCKH>> LayDsChuyenDe()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<ChuyenDeNCKH>>>("api/ChuyenDeNCKH");
            return result?.Data ?? new List<ChuyenDeNCKH>();
        }

        /// <summary>
        /// Lấy danh sách sinh viên (để hiển thị tên người nộp)
        /// </summary>
        public async Task<List<SinhVien>> LayDsSinhVien()
        {
            var result = await _http.GetFromJsonAsyncWithAuth<ApiResult<List<SinhVien>>>("api/SinhVien");
            return result?.Data ?? new List<SinhVien>();
        }
    }
}
