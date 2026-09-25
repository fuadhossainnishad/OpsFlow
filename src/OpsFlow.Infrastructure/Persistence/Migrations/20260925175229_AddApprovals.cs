using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpsFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000002"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000004"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000005"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000007"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000008"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000012"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000013"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000014"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000015"), new Guid("11111111-1111-1111-1111-111111111111") });

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
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000002"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000004"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000005"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000007"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000008"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000012"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000013"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000015"), new Guid("22222222-2222-2222-2222-222222222222") });

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
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000007"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000008"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000012"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000013"), new Guid("33333333-3333-3333-3333-333333333333") });

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
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000012"), new Guid("44444444-4444-4444-4444-444444444444") });

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
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000013"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000015"));

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

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("71000000-0000-0000-0000-000000000023"));

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DecidedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecisionComment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DecidedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_TimeEntries_TimeEntryId",
                        column: x => x.TimeEntryId,
                        principalTable: "TimeEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Users_DecidedByUserId",
                        column: x => x.DecidedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Users_RequesterUserId",
                        column: x => x.RequesterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "organizations.read", "Read organizations" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "organizations.create", "Create organizations" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "organizations.update", "Update organizations" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "members.read", "Read members" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "members.invite", "Invite members" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "members.manage", "Manage members" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "projects.read", "Read projects" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "projects.create", "Create projects" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "projects.update", "Update projects" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "tasks.read", "Read tasks" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "tasks.create", "Create tasks" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "tasks.update", "Update tasks" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "tasks.assign", "Assign tasks" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "reports.read", "Read reports" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "billing.manage", "Manage billing" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "teams.read", "Read teams" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "teams.create", "Create teams" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "teams.update", "Update teams" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "teams.manage_members", "Manage team members" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "time_entries.read", "Read time entries" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "time_entries.create", "Create time entries" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "time_entries.update", "Update time entries" },
                    { new Guid("00000000-0000-0000-0000-000000000023"), "time_entries.delete", "Delete time entries" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000005"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000006"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000008"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000009"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000013"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000014"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000015"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000017"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000018"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000019"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000005"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000006"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000008"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000009"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000013"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000014"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000017"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000018"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000019"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000008"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000009"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000013"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000014"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000017"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000018"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000019"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000013"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000018"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000019"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("00000000-0000-0000-0000-000000000014"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("66666666-6666-6666-6666-666666666666") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_DecidedByUserId",
                table: "ApprovalRequests",
                column: "DecidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_OrganizationId_Status_CreatedAtUtc",
                table: "ApprovalRequests",
                columns: new[] { "OrganizationId", "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_OrganizationId_TimeEntryId",
                table: "ApprovalRequests",
                columns: new[] { "OrganizationId", "TimeEntryId" },
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_RequesterUserId",
                table: "ApprovalRequests",
                column: "RequesterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_TimeEntryId",
                table: "ApprovalRequests",
                column: "TimeEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000002"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000005"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000006"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000008"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000015"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000002"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000005"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000006"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000008"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000008"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("44444444-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000023"), new Guid("55555555-5555-5555-5555-555555555555") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000023"));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("71000000-0000-0000-0000-000000000001"), "organizations.read", "Read organizations" },
                    { new Guid("71000000-0000-0000-0000-000000000002"), "organizations.update", "Update organizations" },
                    { new Guid("71000000-0000-0000-0000-000000000003"), "members.read", "Read members" },
                    { new Guid("71000000-0000-0000-0000-000000000004"), "members.invite", "Invite members" },
                    { new Guid("71000000-0000-0000-0000-000000000005"), "members.manage", "Manage members" },
                    { new Guid("71000000-0000-0000-0000-000000000006"), "projects.read", "Read projects" },
                    { new Guid("71000000-0000-0000-0000-000000000007"), "projects.create", "Create projects" },
                    { new Guid("71000000-0000-0000-0000-000000000008"), "projects.update", "Update projects" },
                    { new Guid("71000000-0000-0000-0000-000000000009"), "tasks.read", "Read tasks" },
                    { new Guid("71000000-0000-0000-0000-000000000010"), "tasks.create", "Create tasks" },
                    { new Guid("71000000-0000-0000-0000-000000000011"), "tasks.update", "Update tasks" },
                    { new Guid("71000000-0000-0000-0000-000000000012"), "tasks.assign", "Assign tasks" },
                    { new Guid("71000000-0000-0000-0000-000000000013"), "reports.read", "Read reports" },
                    { new Guid("71000000-0000-0000-0000-000000000014"), "billing.manage", "Manage billing" },
                    { new Guid("71000000-0000-0000-0000-000000000015"), "organizations.create", "Create organizations" },
                    { new Guid("71000000-0000-0000-0000-000000000016"), "teams.read", "Read teams" },
                    { new Guid("71000000-0000-0000-0000-000000000017"), "teams.create", "Create teams" },
                    { new Guid("71000000-0000-0000-0000-000000000018"), "teams.update", "Update teams" },
                    { new Guid("71000000-0000-0000-0000-000000000019"), "teams.manage_members", "Manage team members" },
                    { new Guid("71000000-0000-0000-0000-000000000020"), "time_entries.read", "Read time entries" },
                    { new Guid("71000000-0000-0000-0000-000000000021"), "time_entries.create", "Create time entries" },
                    { new Guid("71000000-0000-0000-0000-000000000022"), "time_entries.update", "Update time entries" },
                    { new Guid("71000000-0000-0000-0000-000000000023"), "time_entries.delete", "Delete time entries" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000002"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000004"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000005"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000007"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000008"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000012"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000013"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000014"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000015"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000002"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000004"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000005"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000007"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000008"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000012"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000013"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000015"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000007"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000008"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000012"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000013"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000017"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000012"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000010"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000011"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000001"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("71000000-0000-0000-0000-000000000003"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("71000000-0000-0000-0000-000000000006"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("71000000-0000-0000-0000-000000000009"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("71000000-0000-0000-0000-000000000013"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new Guid("66666666-6666-6666-6666-666666666666") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("66666666-6666-6666-6666-666666666666") }
                });
        }
    }
}
