using System.ComponentModel.DataAnnotations;

namespace QLNCKH_HocVien.Client.Models
{
    /// <summary>
    /// Model người dùng hệ thống
    /// </summary>
    public class NguoiDung
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3-50 ký tự")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6-100 ký tự")]
        public string MatKhau { get; set; } = string.Empty;

        public string HoTen { get; set; } = string.Empty;

        public string VaiTro { get; set; } = "User"; // Admin, User, GiaoVien

        public bool IsActive { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Model đăng nhập
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        public bool GhiNho { get; set; } = false;
    }

    /// <summary>
    /// Response sau khi đăng nhập
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// Thông tin user hiển thị
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
    }
}

