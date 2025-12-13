using Microsoft.AspNetCore.Mvc;
using QLNCKH_HocVien.Client.Models;

namespace QLNCKH_HocVien.Helpers
{
    /// <summary>
    /// Extension methods để trả về ApiResult từ Controller
    /// </summary>
    public static class ApiResultExtensions
    {
        /// <summary>
        /// Trả về 200 OK với data
        /// </summary>
        public static ActionResult<ApiResult<T>> OkResult<T>(this ControllerBase controller, T data, string message = "Thành công")
        {
            return controller.Ok(ApiResult<T>.Ok(data, message));
        }

        /// <summary>
        /// Trả về 200 OK không có data
        /// </summary>
        public static ActionResult<ApiResult> OkResult(this ControllerBase controller, string message = "Thành công")
        {
            return controller.Ok(ApiResult.Ok(message));
        }

        /// <summary>
        /// Trả về 201 Created với data
        /// </summary>
        public static ActionResult<ApiResult<T>> CreatedResult<T>(this ControllerBase controller, T data, string message = "Tạo mới thành công")
        {
            return controller.StatusCode(201, ApiResult<T>.Ok(data, message));
        }

        /// <summary>
        /// Trả về 400 Bad Request
        /// </summary>
        public static ActionResult<ApiResult<T>> BadRequestResult<T>(this ControllerBase controller, string message, List<string>? errors = null)
        {
            return controller.BadRequest(ApiResult<T>.Fail(message, errors));
        }

        /// <summary>
        /// Trả về 400 Bad Request không có data type
        /// </summary>
        public static ActionResult<ApiResult> BadRequestResult(this ControllerBase controller, string message, List<string>? errors = null)
        {
            return controller.BadRequest(ApiResult.Fail(message, errors));
        }

        /// <summary>
        /// Trả về 404 Not Found
        /// </summary>
        public static ActionResult<ApiResult<T>> NotFoundResult<T>(this ControllerBase controller, string message)
        {
            return controller.NotFound(ApiResult<T>.Fail(message));
        }

        /// <summary>
        /// Trả về 404 Not Found không có data type
        /// </summary>
        public static ActionResult<ApiResult> NotFoundResult(this ControllerBase controller, string message)
        {
            return controller.NotFound(ApiResult.Fail(message));
        }

        /// <summary>
        /// Trả về PaginatedResult
        /// </summary>
        public static ActionResult<PaginatedResult<T>> PaginatedOk<T>(
            this ControllerBase controller, 
            List<T> items, 
            int totalCount, 
            int pageNumber, 
            int pageSize)
        {
            return controller.Ok(PaginatedResult<T>.Create(items, totalCount, pageNumber, pageSize));
        }
    }
}

