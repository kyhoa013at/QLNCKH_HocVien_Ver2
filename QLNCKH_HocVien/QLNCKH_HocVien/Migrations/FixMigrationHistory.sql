-- Script thêm lịch sử migration cho database đã tồn tại
-- Chạy script này TRƯỚC KHI chạy 'dotnet ef database update'

-- Tạo bảng __EFMigrationsHistory nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END
GO

-- Thêm các migration đã áp dụng (các bảng đã tồn tại trong DB)
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
GO

PRINT 'Migration history updated successfully!';
PRINT 'Now you can run: dotnet ef database update';
GO

