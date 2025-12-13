namespace QLNCKH_HocVien.Client.Exceptions
{
    /// <summary>
    /// Exception khi gặp lỗi 401 Unauthorized
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("Bạn không có quyền truy cập")
        {
        }

        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}

