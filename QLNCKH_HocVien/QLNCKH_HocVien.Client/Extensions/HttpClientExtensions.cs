using System.Net;
using System.Net.Http.Json;
using QLNCKH_HocVien.Client.Exceptions;

namespace QLNCKH_HocVien.Client.Extensions
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Get JSON từ API và tự động throw UnauthorizedException nếu gặp 401
        /// </summary>
        public static async Task<T?> GetFromJsonAsyncWithAuth<T>(this HttpClient httpClient, string? requestUri)
        {
            var response = await httpClient.GetAsync(requestUri);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        /// <summary>
        /// Post JSON và tự động throw UnauthorizedException nếu gặp 401
        /// </summary>
        public static async Task<HttpResponseMessage> PostAsJsonAsyncWithAuth<T>(this HttpClient httpClient, string? requestUri, T value)
        {
            var response = await httpClient.PostAsJsonAsync(requestUri, value);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();
            
            return response;
        }

        /// <summary>
        /// Post không có body và tự động throw UnauthorizedException nếu gặp 401
        /// </summary>
        public static async Task<HttpResponseMessage> PostAsyncWithAuth(this HttpClient httpClient, string? requestUri, HttpContent? content)
        {
            var response = await httpClient.PostAsync(requestUri, content);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();
            
            return response;
        }

        /// <summary>
        /// Put JSON và tự động throw UnauthorizedException nếu gặp 401
        /// </summary>
        public static async Task<HttpResponseMessage> PutAsJsonAsyncWithAuth<T>(this HttpClient httpClient, string? requestUri, T value)
        {
            var response = await httpClient.PutAsJsonAsync(requestUri, value);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();
            
            return response;
        }

        /// <summary>
        /// Delete và tự động throw UnauthorizedException nếu gặp 401
        /// </summary>
        public static async Task<HttpResponseMessage> DeleteAsyncWithAuth(this HttpClient httpClient, string? requestUri)
        {
            var response = await httpClient.DeleteAsync(requestUri);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();
            
            return response;
        }
    }
}
