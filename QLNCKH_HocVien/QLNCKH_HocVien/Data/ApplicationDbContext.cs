using Microsoft.EntityFrameworkCore;
using QLNCKH_HocVien.Client.Models;

namespace QLNCKH_HocVien.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Bảng người dùng
        public DbSet<NguoiDung> NguoiDungs { get; set; }

        // Bảng SinhVien
        public DbSet<SinhVien> SinhViens { get; set; }

        // Bảng GiaoVien
        public DbSet<GiaoVien> GiaoViens { get; set; }

        // Bảng ChuyenDeNCKH
        public DbSet<ChuyenDeNCKH> ChuyenDeNCKHs { get; set; }

        // Bảng NopSanPham
        public DbSet<NopSanPham> NopSanPhams { get; set; }

        // Bảng HoiDong
        public DbSet<HoiDong> HoiDongs { get; set; }

        // Bảng ThanhVienHoiDong
        public DbSet<ThanhVienHoiDong> ThanhVienHoiDongs { get; set; }

        // Bảng KetQuaSoLoai
        public DbSet<KetQuaSoLoai> KetQuaSoLoais { get; set; }

        // Bảng PhieuCham
        public DbSet<PhieuCham> PhieuChams { get; set; }

        // Bảng XepGiai
        public DbSet<XepGiai> XepGiais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== NGƯỜI DÙNG ====================
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.HasIndex(u => u.TenDangNhap).IsUnique();
                entity.Property(u => u.TenDangNhap).HasMaxLength(50);
                entity.Property(u => u.MatKhau).HasMaxLength(100);
                entity.Property(u => u.HoTen).HasMaxLength(100);
                entity.Property(u => u.VaiTro).HasMaxLength(20);
            });

            // ==================== SINH VIÊN ====================
            modelBuilder.Entity<SinhVien>(entity =>
            {
                entity.HasIndex(s => s.MaSV).IsUnique();
                entity.Property(s => s.MaSV).HasMaxLength(20);
                entity.Property(s => s.HoTen).HasMaxLength(100);
                entity.Property(s => s.GioiTinh).HasMaxLength(10);
                entity.Property(s => s.SoDienThoai).HasMaxLength(15);
                entity.Property(s => s.Lop).HasMaxLength(50);
                entity.Property(s => s.ChuyenNganh).HasMaxLength(100);
            });

            // ==================== GIÁO VIÊN ====================
            modelBuilder.Entity<GiaoVien>(entity =>
            {
                entity.HasIndex(g => g.MaSoCB).IsUnique();
                entity.Property(g => g.MaSoCB).HasMaxLength(20);
                entity.Property(g => g.HoTen).HasMaxLength(100);
                entity.Property(g => g.GioiTinh).HasMaxLength(10);
                entity.Property(g => g.SoDienThoai).HasMaxLength(15);
                entity.Property(g => g.LinhVuc).HasMaxLength(200);
            });

            // ==================== CHUYÊN ĐỀ NCKH ====================
            modelBuilder.Entity<ChuyenDeNCKH>(entity =>
            {
                entity.HasIndex(c => c.MaSoCD).IsUnique();
                entity.Property(c => c.MaSoCD).HasMaxLength(20);
                entity.Property(c => c.TenChuyenDe).HasMaxLength(500);

                // Relationship: ChuyenDe -> SinhVien (Many-to-One)
                entity.HasOne<SinhVien>()
                    .WithMany()
                    .HasForeignKey(c => c.IdHocVien)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index for filtering
                entity.HasIndex(c => c.IdLinhVuc);
                entity.HasIndex(c => c.IdHocVien);
            });

            // ==================== NỘP SẢN PHẨM ====================
            modelBuilder.Entity<NopSanPham>(entity =>
            {
                entity.Property(n => n.GhiChu).HasMaxLength(500);

                // Relationship: NopSanPham -> ChuyenDe (Many-to-One)
                entity.HasOne<ChuyenDeNCKH>()
                    .WithMany()
                    .HasForeignKey(n => n.IdChuyenDe)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index for filtering
                entity.HasIndex(n => n.IdChuyenDe);
                entity.HasIndex(n => n.TrangThai);
                entity.HasIndex(n => n.NgayNop);
            });

            // ==================== HỘI ĐỒNG ====================
            modelBuilder.Entity<HoiDong>(entity =>
            {
                entity.Property(h => h.DiaDiem).HasMaxLength(200);

                // Relationship: HoiDong -> ChuyenDe (Many-to-One)
                entity.HasOne<ChuyenDeNCKH>()
                    .WithMany()
                    .HasForeignKey(h => h.IdChuyenDe)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship: HoiDong -> ThanhVienHoiDong (One-to-Many)
                entity.HasMany(h => h.ThanhViens)
                    .WithOne()
                    .HasForeignKey(t => t.IdHoiDong)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index for filtering
                entity.HasIndex(h => h.IdChuyenDe);
                entity.HasIndex(h => h.VongThi);
                entity.HasIndex(h => h.NgayCham);
                
                // Unique: Mỗi chuyên đề chỉ có 1 hội đồng cho mỗi vòng
                entity.HasIndex(h => new { h.IdChuyenDe, h.VongThi }).IsUnique();
            });

            // ==================== THÀNH VIÊN HỘI ĐỒNG ====================
            modelBuilder.Entity<ThanhVienHoiDong>(entity =>
            {
                entity.Property(t => t.VaiTro).HasMaxLength(50);

                // Relationship: ThanhVien -> GiaoVien (Many-to-One)
                entity.HasOne<GiaoVien>()
                    .WithMany()
                    .HasForeignKey(t => t.IdGiaoVien)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index for filtering
                entity.HasIndex(t => t.IdHoiDong);
                entity.HasIndex(t => t.IdGiaoVien);

                // Unique: Mỗi giáo viên chỉ tham gia 1 lần trong 1 hội đồng
                entity.HasIndex(t => new { t.IdHoiDong, t.IdGiaoVien }).IsUnique();
            });

            // ==================== KẾT QUẢ SƠ LOẠI ====================
            modelBuilder.Entity<KetQuaSoLoai>(entity =>
            {
                entity.Property(k => k.NhanXet).HasMaxLength(500);
                entity.Property(k => k.DiemSo).HasPrecision(4, 2);

                // Relationship: KetQuaSoLoai -> ChuyenDe (One-to-One)
                entity.HasOne<ChuyenDeNCKH>()
                    .WithOne()
                    .HasForeignKey<KetQuaSoLoai>(k => k.IdChuyenDe)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index
                entity.HasIndex(k => k.IdChuyenDe).IsUnique();
                entity.HasIndex(k => k.KetQua);
                entity.HasIndex(k => k.DiemSo);
            });

            // ==================== PHIẾU CHẤM ====================
            modelBuilder.Entity<PhieuCham>(entity =>
            {
                entity.Property(p => p.YKien).HasMaxLength(500);
                entity.Property(p => p.Diem).HasPrecision(4, 2);

                // Relationship: PhieuCham -> ChuyenDe (Many-to-One)
                entity.HasOne<ChuyenDeNCKH>()
                    .WithMany()
                    .HasForeignKey(p => p.IdChuyenDe)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship: PhieuCham -> GiaoVien (Many-to-One)
                entity.HasOne<GiaoVien>()
                    .WithMany()
                    .HasForeignKey(p => p.IdGiaoVien)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index
                entity.HasIndex(p => p.IdChuyenDe);
                entity.HasIndex(p => p.IdGiaoVien);
                
                // Unique: Mỗi giáo viên chỉ chấm 1 lần cho 1 chuyên đề
                entity.HasIndex(p => new { p.IdChuyenDe, p.IdGiaoVien }).IsUnique();
            });

            // ==================== XẾP GIẢI ====================
            modelBuilder.Entity<XepGiai>(entity =>
            {
                entity.Property(x => x.TenGiai).HasMaxLength(50);
                entity.Property(x => x.DiemTrungBinh).HasPrecision(4, 2);

                // Relationship: XepGiai -> ChuyenDe (One-to-One)
                entity.HasOne<ChuyenDeNCKH>()
                    .WithOne()
                    .HasForeignKey<XepGiai>(x => x.IdChuyenDe)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index
                entity.HasIndex(x => x.IdChuyenDe).IsUnique();
                entity.HasIndex(x => x.XepHang);
                entity.HasIndex(x => x.TenGiai);
            });

            // ==================== SEED DATA ====================
            // Seed admin user mặc định (password: Admin@123)
            modelBuilder.Entity<NguoiDung>().HasData(new NguoiDung
            {
                Id = 1,
                TenDangNhap = "admin",
                MatKhau = "o8dJmVSf+0e3kEpbSfbF0DP3lmfKGTG8FhCXJ0kbJbY=",
                HoTen = "Quản trị viên",
                VaiTro = "Admin",
                IsActive = true,
                NgayTao = new DateTime(2024, 1, 1)
            });
        }
    }
}
