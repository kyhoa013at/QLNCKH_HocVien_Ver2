-- Script tạo user Admin mặc định
-- Password: Admin@123 (đã hash với SHA256 + salt)

-- Kiểm tra nếu chưa có user admin thì tạo mới
IF NOT EXISTS (SELECT 1 FROM NguoiDungs WHERE TenDangNhap = 'admin')
BEGIN
    INSERT INTO NguoiDungs (TenDangNhap, MatKhau, HoTen, VaiTro, IsActive, NgayTao)
    VALUES (
        'admin',
        'o8dJmVSf+0e3kEpbSfbF0DP3lmfKGTG8FhCXJ0kbJbY=', -- Hash của 'Admin@123' + QLNCKH_SALT_2024
        N'Quản trị viên',
        'Admin',
        1,
        GETDATE()
    );
    PRINT 'Created admin user successfully';
END
ELSE
BEGIN
    PRINT 'Admin user already exists';
END
GO

