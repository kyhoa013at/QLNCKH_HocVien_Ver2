using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNCKH_HocVien.Client.Models;
using QLNCKH_HocVien.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QLNCKH_HocVien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Đăng nhập (API - trả về JSON)
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TenDangNhap) || string.IsNullOrWhiteSpace(request.MatKhau))
            {
                return BadRequest(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Vui lòng nhập tên đăng nhập và mật khẩu" 
                });
            }

            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap == request.TenDangNhap && u.IsActive);

            if (user == null)
            {
                return Unauthorized(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng" 
                });
            }

            // Verify password
            if (!VerifyPassword(request.MatKhau, user.MatKhau))
            {
                return Unauthorized(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng" 
                });
            }

            // Tạo claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim(ClaimTypes.GivenName, user.HoTen),
                new Claim(ClaimTypes.Role, user.VaiTro)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = request.GhiNho,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(request.GhiNho ? 24 * 7 : 8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                User = new UserInfo
                {
                    Id = user.Id,
                    TenDangNhap = user.TenDangNhap,
                    HoTen = user.HoTen,
                    VaiTro = user.VaiTro
                }
            });
        }

        /// <summary>
        /// Đăng nhập (Form POST - set cookie cho browser)
        /// </summary>
        [HttpPost("login-form")]
        public async Task<IActionResult> LoginForm([FromForm] string username, [FromForm] string password, [FromForm] bool remember = false)
        {
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap == username && u.IsActive);

            if (user == null || !VerifyPassword(password, user.MatKhau))
            {
                return Redirect("/login?error=1");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim(ClaimTypes.GivenName, user.HoTen),
                new Claim(ClaimTypes.Role, user.VaiTro)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                principal,
                new AuthenticationProperties { IsPersistent = remember });

            return Redirect("/");
        }

        /// <summary>
        /// Đăng xuất (API)
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Đăng xuất thành công" });
        }

        /// <summary>
        /// Đăng xuất (GET - redirect về login)
        /// </summary>
        [HttpGet("logout")]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/login");
        }

        /// <summary>
        /// Lấy thông tin user hiện tại
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<UserInfo> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var hoTen = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var vaiTro = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new UserInfo
            {
                Id = int.Parse(userId ?? "0"),
                TenDangNhap = userName ?? "",
                HoTen = hoTen ?? "",
                VaiTro = vaiTro ?? ""
            });
        }

        /// <summary>
        /// Kiểm tra trạng thái đăng nhập
        /// </summary>
        [HttpGet("check")]
        public ActionResult<object> CheckAuth()
        {
            return Ok(new 
            { 
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                UserName = User.Identity?.Name
            });
        }

        /// <summary>
        /// Khởi tạo/Reset password admin (chỉ dùng lần đầu)
        /// </summary>
        [HttpGet("init-admin")]
        [HttpPost("init-admin")]
        public async Task<IActionResult> InitAdmin()
        {
            var admin = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == "admin");
            
            if (admin == null)
            {
                // Tạo mới admin
                admin = new NguoiDung
                {
                    TenDangNhap = "admin",
                    MatKhau = HashPassword("Admin@123"),
                    HoTen = "Quản trị viên",
                    VaiTro = "Admin",
                    IsActive = true,
                    NgayTao = DateTime.Now
                };
                _context.NguoiDungs.Add(admin);
            }
            else
            {
                // Reset password về mặc định
                admin.MatKhau = HashPassword("Admin@123");
            }
            
            await _context.SaveChangesAsync();
            
            return Ok(new 
            { 
                Message = "Đã khởi tạo tài khoản admin!", 
                Username = "admin", 
                Password = "Admin@123" 
            });
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user == null)
                return NotFound(new { Message = "Không tìm thấy người dùng" });

            if (!VerifyPassword(request.MatKhauCu, user.MatKhau))
                return BadRequest(new { Message = "Mật khẩu cũ không đúng" });

            if (request.MatKhauMoi.Length < 6)
                return BadRequest(new { Message = "Mật khẩu mới phải có ít nhất 6 ký tự" });

            user.MatKhau = HashPassword(request.MatKhauMoi);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đổi mật khẩu thành công" });
        }

        /// <summary>
        /// Tạo user mới (Admin only)
        /// </summary>
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(NguoiDung user)
        {
            if (await _context.NguoiDungs.AnyAsync(u => u.TenDangNhap == user.TenDangNhap))
                return BadRequest(new { Message = "Tên đăng nhập đã tồn tại" });

            user.MatKhau = HashPassword(user.MatKhau);
            user.NgayTao = DateTime.Now;

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Tạo tài khoản thành công" });
        }

        // ========== HELPER METHODS ==========

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "QLNCKH_SALT_2024"));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }
    }

    public class ChangePasswordRequest
    {
        public string MatKhauCu { get; set; } = string.Empty;
        public string MatKhauMoi { get; set; } = string.Empty;
    }
}

