using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNCKH_HocVien.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThanhVienHoiDongs_HoiDongs_HoiDongId",
                table: "ThanhVienHoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_ThanhVienHoiDongs_HoiDongId",
                table: "ThanhVienHoiDongs");

            migrationBuilder.DropColumn(
                name: "HoiDongId",
                table: "ThanhVienHoiDongs");

            migrationBuilder.AlterColumn<string>(
                name: "MaSV",
                table: "SinhViens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MaSoCB",
                table: "GiaoViens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MaSoCD",
                table: "ChuyenDeNCKHs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "NguoiDungs",
                columns: new[] { "Id", "HoTen", "IsActive", "MatKhau", "NgayTao", "TenDangNhap", "VaiTro" },
                values: new object[] { 1, "Quản trị viên", true, "o8dJmVSf+0e3kEpbSfbF0DP3lmfKGTG8FhCXJ0kbJbY=", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienHoiDongs_IdHoiDong",
                table: "ThanhVienHoiDongs",
                column: "IdHoiDong");

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_MaSV",
                table: "SinhViens",
                column: "MaSV",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GiaoViens_MaSoCB",
                table: "GiaoViens",
                column: "MaSoCB",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDeNCKHs_MaSoCD",
                table: "ChuyenDeNCKHs",
                column: "MaSoCD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_TenDangNhap",
                table: "NguoiDungs",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhVienHoiDongs_HoiDongs_IdHoiDong",
                table: "ThanhVienHoiDongs",
                column: "IdHoiDong",
                principalTable: "HoiDongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThanhVienHoiDongs_HoiDongs_IdHoiDong",
                table: "ThanhVienHoiDongs");

            migrationBuilder.DropTable(
                name: "NguoiDungs");

            migrationBuilder.DropIndex(
                name: "IX_ThanhVienHoiDongs_IdHoiDong",
                table: "ThanhVienHoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_SinhViens_MaSV",
                table: "SinhViens");

            migrationBuilder.DropIndex(
                name: "IX_GiaoViens_MaSoCB",
                table: "GiaoViens");

            migrationBuilder.DropIndex(
                name: "IX_ChuyenDeNCKHs_MaSoCD",
                table: "ChuyenDeNCKHs");

            migrationBuilder.AddColumn<int>(
                name: "HoiDongId",
                table: "ThanhVienHoiDongs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaSV",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MaSoCB",
                table: "GiaoViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MaSoCD",
                table: "ChuyenDeNCKHs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienHoiDongs_HoiDongId",
                table: "ThanhVienHoiDongs",
                column: "HoiDongId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhVienHoiDongs_HoiDongs_HoiDongId",
                table: "ThanhVienHoiDongs",
                column: "HoiDongId",
                principalTable: "HoiDongs",
                principalColumn: "Id");
        }
    }
}
