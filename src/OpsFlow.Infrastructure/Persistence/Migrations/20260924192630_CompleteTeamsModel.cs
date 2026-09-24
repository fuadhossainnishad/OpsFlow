using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpsFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteTeamsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("71000000-0000-0000-0000-000000000016"), "teams.read", "Read teams" },
                    { new Guid("71000000-0000-0000-0000-000000000017"), "teams.create", "Create teams" },
                    { new Guid("71000000-0000-0000-0000-000000000018"), "teams.update", "Update teams" },
                    { new Guid("71000000-0000-0000-0000-000000000019"), "teams.manage_members", "Manage team members" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("66666666-6666-6666-6666-666666666666") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000019"));
        }
    }
}
