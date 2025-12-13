-- =====================================================
-- SCRIPT TẠO BẢNG NGUOIDUNGS VÀ USER ADMIN
-- Chạy script này trên SQL Server Management Studio
-- hoặc Azure Data Studio
-- =====================================================

-- 1. Tạo bảng NguoiDungs (nếu chưa có)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NguoiDungs')
BEGIN
    CREATE TABLE [NguoiDungs] (
        [Id] int NOT NULL IDENTITY(1,1),
        [TenDangNhap] nvarchar(50) NOT NULL,
        [MatKhau] nvarchar(100) NOT NULL,
        [HoTen] nvarchar(max) NOT NULL,
        [VaiTro] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [NgayTao] datetime2 NOT NULL,
        CONSTRAINT [PK_NguoiDungs] PRIMARY KEY ([Id])
    );
    
    -- Tạo unique index cho TenDangNhap
    CREATE UNIQUE INDEX [IX_NguoiDungs_TenDangNhap] ON [NguoiDungs] ([TenDangNhap]);
    
    PRINT N'✅ Đã tạo bảng NguoiDungs thành công!';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng NguoiDungs đã tồn tại.';
END
GO

-- 2. Tạo unique index cho các bảng khác (nếu chưa có)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SinhViens_MaSV')
BEGIN
    CREATE UNIQUE INDEX [IX_SinhViens_MaSV] ON [SinhViens] ([MaSV]) WHERE [MaSV] IS NOT NULL;
    PRINT N'✅ Đã tạo index cho MaSV';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GiaoViens_MaSoCB')
BEGIN
    CREATE UNIQUE INDEX [IX_GiaoViens_MaSoCB] ON [GiaoViens] ([MaSoCB]) WHERE [MaSoCB] IS NOT NULL;
    PRINT N'✅ Đã tạo index cho MaSoCB';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChuyenDeNCKHs_MaSoCD')
BEGIN
    CREATE UNIQUE INDEX [IX_ChuyenDeNCKHs_MaSoCD] ON [ChuyenDeNCKHs] ([MaSoCD]) WHERE [MaSoCD] IS NOT NULL;
    PRINT N'✅ Đã tạo index cho MaSoCD';
END
GO

-- 3. Tạo user Admin mặc định
-- Password: Admin@123 (đã hash với SHA256 + salt "QLNCKH_SALT_2024")
IF NOT EXISTS (SELECT 1 FROM NguoiDungs WHERE TenDangNhap = 'admin')
BEGIN
    INSERT INTO NguoiDungs (TenDangNhap, MatKhau, HoTen, VaiTro, IsActive, NgayTao)
    VALUES (
        'admin',
        'o8dJmVSf+0e3kEpbSfbF0DP3lmfKGTG8FhCXJ0kbJbY=',
        N'Quản trị viên',
        'Admin',
        1,
        GETDATE()
    );
    PRINT N'✅ Đã tạo tài khoản admin thành công!';
    PRINT N'   Username: admin';
    PRINT N'   Password: Admin@123';
END
ELSE
BEGIN
    PRINT N'⚠️ Tài khoản admin đã tồn tại.';
END
GO

-- 4. Thêm lịch sử Migration (để EF Core không cố chạy lại)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT N'✅ Đã tạo bảng __EFMigrationsHistory';
END
GO

-- Thêm các migration đã áp dụng
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE MigrationId = '20251201161039_FirstMigration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES 
        ('20251201161039_FirstMigration', '8.0.22'),
        ('20251201163830_ChoPhepNullCacTruong', '8.0.22'),
        ('20251201165256_TaoBangGiaoVien_Chuan', '8.0.22'),
        ('20251201172219_TaoBangChuyenDe', '8.0.22'),
        ('20251201174720_TaoBangNopSanPham', '8.0.22'),
        ('20251202020554_TaoBangHoiDong', '8.0.22'),
        ('20251202025204_TaoBangKetQua', '8.0.22'),
        ('20251202030143_TaoBangXepGiai', '8.0.22');
    PRINT N'✅ Đã cập nhật lịch sử migration';
END
GO

PRINT N'';
PRINT N'====================================';
PRINT N'✅ HOÀN TẤT! Bạn có thể chạy ứng dụng';
PRINT N'====================================';
GO

