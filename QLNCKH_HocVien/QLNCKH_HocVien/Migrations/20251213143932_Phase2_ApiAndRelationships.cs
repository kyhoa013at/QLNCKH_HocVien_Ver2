using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNCKH_HocVien.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_ApiAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TenGiai",
                table: "XepGiais",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "DiemTrungBinh",
                table: "XepGiais",
                type: "float(4)",
                precision: 4,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "ThanhVienHoiDongs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "SinhViens",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaSV",
                table: "SinhViens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Lop",
                table: "SinhViens",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "SinhViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinh",
                table: "SinhViens",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ChuyenNganh",
                table: "SinhViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "YKien",
                table: "PhieuChams",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Diem",
                table: "PhieuChams",
                type: "float(4)",
                precision: 4,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "NopSanPhams",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "NguoiDungs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "NguoiDungs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NhanXet",
                table: "KetQuaSoLoais",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DiemSo",
                table: "KetQuaSoLoais",
                type: "float(4)",
                precision: 4,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "DiaDiem",
                table: "HoiDongs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "GiaoViens",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaSoCB",
                table: "GiaoViens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LinhVuc",
                table: "GiaoViens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "GiaoViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinh",
                table: "GiaoViens",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TenChuyenDe",
                table: "ChuyenDeNCKHs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MaSoCD",
                table: "ChuyenDeNCKHs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_XepGiais_IdChuyenDe",
                table: "XepGiais",
                column: "IdChuyenDe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_XepGiais_TenGiai",
                table: "XepGiais",
                column: "TenGiai");

            migrationBuilder.CreateIndex(
                name: "IX_XepGiais_XepHang",
                table: "XepGiais",
                column: "XepHang");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienHoiDongs_IdGiaoVien",
                table: "ThanhVienHoiDongs",
                column: "IdGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienHoiDongs_IdHoiDong_IdGiaoVien",
                table: "ThanhVienHoiDongs",
                columns: new[] { "IdHoiDong", "IdGiaoVien" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuChams_IdChuyenDe",
                table: "PhieuChams",
                column: "IdChuyenDe");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuChams_IdChuyenDe_IdGiaoVien",
                table: "PhieuChams",
                columns: new[] { "IdChuyenDe", "IdGiaoVien" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuChams_IdGiaoVien",
                table: "PhieuChams",
                column: "IdGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_NopSanPhams_IdChuyenDe",
                table: "NopSanPhams",
                column: "IdChuyenDe");

            migrationBuilder.CreateIndex(
                name: "IX_NopSanPhams_NgayNop",
                table: "NopSanPhams",
                column: "NgayNop");

            migrationBuilder.CreateIndex(
                name: "IX_NopSanPhams_TrangThai",
                table: "NopSanPhams",
                column: "TrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaSoLoais_DiemSo",
                table: "KetQuaSoLoais",
                column: "DiemSo");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaSoLoais_IdChuyenDe",
                table: "KetQuaSoLoais",
                column: "IdChuyenDe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaSoLoais_KetQua",
                table: "KetQuaSoLoais",
                column: "KetQua");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_IdChuyenDe",
                table: "HoiDongs",
                column: "IdChuyenDe");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_IdChuyenDe_VongThi",
                table: "HoiDongs",
                columns: new[] { "IdChuyenDe", "VongThi" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_NgayCham",
                table: "HoiDongs",
                column: "NgayCham");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_VongThi",
                table: "HoiDongs",
                column: "VongThi");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDeNCKHs_IdHocVien",
                table: "ChuyenDeNCKHs",
                column: "IdHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDeNCKHs_IdLinhVuc",
                table: "ChuyenDeNCKHs",
                column: "IdLinhVuc");

            migrationBuilder.AddForeignKey(
                name: "FK_ChuyenDeNCKHs_SinhViens_IdHocVien",
                table: "ChuyenDeNCKHs",
                column: "IdHocVien",
                principalTable: "SinhViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoiDongs_ChuyenDeNCKHs_IdChuyenDe",
                table: "HoiDongs",
                column: "IdChuyenDe",
                principalTable: "ChuyenDeNCKHs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KetQuaSoLoais_ChuyenDeNCKHs_IdChuyenDe",
                table: "KetQuaSoLoais",
                column: "IdChuyenDe",
                principalTable: "ChuyenDeNCKHs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NopSanPhams_ChuyenDeNCKHs_IdChuyenDe",
                table: "NopSanPhams",
                column: "IdChuyenDe",
                principalTable: "ChuyenDeNCKHs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuChams_ChuyenDeNCKHs_IdChuyenDe",
                table: "PhieuChams",
                column: "IdChuyenDe",
                principalTable: "ChuyenDeNCKHs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuChams_GiaoViens_IdGiaoVien",
                table: "PhieuChams",
                column: "IdGiaoVien",
                principalTable: "GiaoViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhVienHoiDongs_GiaoViens_IdGiaoVien",
                table: "ThanhVienHoiDongs",
                column: "IdGiaoVien",
                principalTable: "GiaoViens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_XepGiais_ChuyenDeNCKHs_IdChuyenDe",
                table: "XepGiais",
                column: "IdChuyenDe",
                principalTable: "ChuyenDeNCKHs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChuyenDeNCKHs_SinhViens_IdHocVien",
                table: "ChuyenDeNCKHs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoiDongs_ChuyenDeNCKHs_IdChuyenDe",
                table: "HoiDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_KetQuaSoLoais_ChuyenDeNCKHs_IdChuyenDe",
                table: "KetQuaSoLoais");

            migrationBuilder.DropForeignKey(
                name: "FK_NopSanPhams_ChuyenDeNCKHs_IdChuyenDe",
                table: "NopSanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuChams_ChuyenDeNCKHs_IdChuyenDe",
                table: "PhieuChams");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuChams_GiaoViens_IdGiaoVien",
                table: "PhieuChams");

            migrationBuilder.DropForeignKey(
                name: "FK_ThanhVienHoiDongs_GiaoViens_IdGiaoVien",
                table: "ThanhVienHoiDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_XepGiais_ChuyenDeNCKHs_IdChuyenDe",
                table: "XepGiais");

            migrationBuilder.DropIndex(
                name: "IX_XepGiais_IdChuyenDe",
                table: "XepGiais");

            migrationBuilder.DropIndex(
                name: "IX_XepGiais_TenGiai",
                table: "XepGiais");

            migrationBuilder.DropIndex(
                name: "IX_XepGiais_XepHang",
                table: "XepGiais");

            migrationBuilder.DropIndex(
                name: "IX_ThanhVienHoiDongs_IdGiaoVien",
                table: "ThanhVienHoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_ThanhVienHoiDongs_IdHoiDong_IdGiaoVien",
                table: "ThanhVienHoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_PhieuChams_IdChuyenDe",
                table: "PhieuChams");

            migrationBuilder.DropIndex(
                name: "IX_PhieuChams_IdChuyenDe_IdGiaoVien",
                table: "PhieuChams");

            migrationBuilder.DropIndex(
                name: "IX_PhieuChams_IdGiaoVien",
                table: "PhieuChams");

            migrationBuilder.DropIndex(
                name: "IX_NopSanPhams_IdChuyenDe",
                table: "NopSanPhams");

            migrationBuilder.DropIndex(
                name: "IX_NopSanPhams_NgayNop",
                table: "NopSanPhams");

            migrationBuilder.DropIndex(
                name: "IX_NopSanPhams_TrangThai",
                table: "NopSanPhams");

            migrationBuilder.DropIndex(
                name: "IX_KetQuaSoLoais_DiemSo",
                table: "KetQuaSoLoais");

            migrationBuilder.DropIndex(
                name: "IX_KetQuaSoLoais_IdChuyenDe",
                table: "KetQuaSoLoais");

            migrationBuilder.DropIndex(
                name: "IX_KetQuaSoLoais_KetQua",
                table: "KetQuaSoLoais");

            migrationBuilder.DropIndex(
                name: "IX_HoiDongs_IdChuyenDe",
                table: "HoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoiDongs_IdChuyenDe_VongThi",
                table: "HoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoiDongs_NgayCham",
                table: "HoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoiDongs_VongThi",
                table: "HoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_ChuyenDeNCKHs_IdHocVien",
                table: "ChuyenDeNCKHs");

            migrationBuilder.DropIndex(
                name: "IX_ChuyenDeNCKHs_IdLinhVuc",
                table: "ChuyenDeNCKHs");

            migrationBuilder.AlterColumn<string>(
                name: "TenGiai",
                table: "XepGiais",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<double>(
                name: "DiemTrungBinh",
                table: "XepGiais",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(4)",
                oldPrecision: 4,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "ThanhVienHoiDongs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaSV",
                table: "SinhViens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Lop",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinh",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "ChuyenNganh",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "YKien",
                table: "PhieuChams",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Diem",
                table: "PhieuChams",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(4)",
                oldPrecision: 4,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "NopSanPhams",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "NguoiDungs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "NguoiDungs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NhanXet",
                table: "KetQuaSoLoais",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DiemSo",
                table: "KetQuaSoLoais",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(4)",
                oldPrecision: 4,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "DiaDiem",
                table: "HoiDongs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "GiaoViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaSoCB",
                table: "GiaoViens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "LinhVuc",
                table: "GiaoViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "GiaoViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinh",
                table: "GiaoViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "TenChuyenDe",
                table: "ChuyenDeNCKHs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "MaSoCD",
                table: "ChuyenDeNCKHs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
