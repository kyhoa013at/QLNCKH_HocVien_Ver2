-- =====================================================
-- SCRIPT RESET PASSWORD ADMIN
-- Password mới: Admin@123
-- Hash = SHA256("Admin@123" + "QLNCKH_SALT_2024") 
-- =====================================================

-- Cập nhật password cho admin
UPDATE NguoiDungs 
SET MatKhau = 'o8dJmVSf+0e3kEpbSfbF0DP3lmfKGTG8FhCXJ0kbJbY='
WHERE TenDangNhap = 'admin';

-- Kiểm tra kết quả
SELECT Id, TenDangNhap, HoTen, VaiTro, IsActive 
FROM NguoiDungs 
WHERE TenDangNhap = 'admin';

PRINT N'';
PRINT N'✅ Đã reset password admin!';
PRINT N'   Username: admin';
PRINT N'   Password: Admin@123';
GO

