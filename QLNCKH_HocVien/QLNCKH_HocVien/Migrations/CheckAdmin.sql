-- Kiểm tra user admin trong database
SELECT * FROM NguoiDungs WHERE TenDangNhap = 'admin';

-- Xóa user admin cũ và tạo mới với password đúng
-- (Vì hash được tính trong C#, ta cần xóa và để app tự tạo lại)

DELETE FROM NguoiDungs WHERE TenDangNhap = 'admin';

PRINT N'✅ Đã xóa user admin cũ.';
PRINT N'Bây giờ restart app và truy cập: /api/auth/init-admin để tạo lại';
GO

