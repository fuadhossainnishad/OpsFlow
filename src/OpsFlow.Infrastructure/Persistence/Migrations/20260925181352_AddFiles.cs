using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpsFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Files_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), "files.read", "Read files" },
                    { new Guid("77777777-7777-7777-7777-777777777778"), "files.create", "Create files" },
                    { new Guid("77777777-7777-7777-7777-777777777779"), "files.delete", "Delete files" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("77777777-7777-7777-7777-777777777779"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("77777777-7777-7777-7777-777777777779"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("77777777-7777-7777-7777-777777777779"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("77777777-7777-7777-7777-777777777779"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("66666666-6666-6666-6666-666666666666") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_OrganizationId_CreatedAtUtc",
                table: "Files",
                columns: new[] { "OrganizationId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Files_StorageKey",
                table: "Files",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_UploadedByUserId",
                table: "Files",
                column: "UploadedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777779"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777779"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777779"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777779"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777778"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777778"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777779"));
        }
    }
}
