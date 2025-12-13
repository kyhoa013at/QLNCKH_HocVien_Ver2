namespace QLNCKH_HocVien.Client.Models
{
    /// <summary>
    /// Chuẩn hóa response từ API
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        // Factory methods
        public static ApiResult<T> Ok(T data, string message = "Thành công")
        {
            return new ApiResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResult<T> Fail(string message, List<string>? errors = null)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    /// <summary>
    /// Response không có data (chỉ message)
    /// </summary>
    public class ApiResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static ApiResult Ok(string message = "Thành công")
        {
            return new ApiResult { Success = true, Message = message };
        }

        public static ApiResult Fail(string message, List<string>? errors = null)
        {
            return new ApiResult
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    /// <summary>
    /// Response có phân trang
    /// </summary>
    public class PaginatedResult<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Thành công";
        public List<T> Data { get; set; } = new();
        
        // Pagination info
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public static PaginatedResult<T> Create(List<T> items, int count, int pageNumber, int pageSize)
        {
            return new PaginatedResult<T>
            {
                Data = items,
                TotalCount = count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }

    /// <summary>
    /// Request phân trang
    /// </summary>
    public class PaginationRequest
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private const int MaxPageSize = 100;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
        }

        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
}

