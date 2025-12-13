using System.Net.Http.Json;
using QLNCKH_HocVien.Client.Models;

namespace QLNCKH_HocVien.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Auth/login", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponse>() 
                        ?? new LoginResponse { Success = false, Message = "Lỗi không xác định" };
                }

                var errorResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return errorResponse ?? new LoginResponse 
                { 
                    Success = false, 
                    Message = $"Lỗi: {response.StatusCode}" 
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse 
                { 
                    Success = false, 
                    Message = $"Không thể kết nối đến server: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        public async Task<bool> Logout()
        {
            try
            {
                var response = await _http.PostAsync("api/Auth/logout", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin user hiện tại
        /// </summary>
        public async Task<UserInfo?> GetCurrentUser()
        {
            try
            {
                return await _http.GetFromJsonAsync<UserInfo>("api/Auth/me");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra đã đăng nhập chưa
        /// </summary>
        public async Task<bool> IsAuthenticated()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<AuthCheckResult>("api/Auth/check");
                return result?.IsAuthenticated ?? false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public async Task<(bool Success, string Message)> ChangePassword(string oldPassword, string newPassword)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Auth/change-password", new 
                { 
                    MatKhauCu = oldPassword, 
                    MatKhauMoi = newPassword 
                });

                var result = await response.Content.ReadFromJsonAsync<ApiMessage>();
                return (response.IsSuccessStatusCode, result?.Message ?? "Lỗi không xác định");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private class AuthCheckResult
        {
            public bool IsAuthenticated { get; set; }
            public string? UserName { get; set; }
        }

        private class ApiMessage
        {
            public string Message { get; set; } = string.Empty;
        }
    }
}

