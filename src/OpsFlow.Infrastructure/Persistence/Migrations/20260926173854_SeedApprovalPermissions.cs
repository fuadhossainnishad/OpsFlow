using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpsFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedApprovalPermissions : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.InsertData(
        table: "Permissions",
        columns: new[] { "Id", "Code", "Name" },
        values: new object[,]
        {
            {
                new Guid("77777777-7777-7777-7777-777777777771"),
                "approvals.read",
                "Read approvals"
            },
            {
                new Guid("77777777-7777-7777-7777-777777777772"),
                "approvals.create",
                "Create approvals"
            },
            {
                new Guid("77777777-7777-7777-7777-777777777773"),
                "approvals.approve",
                "Approve approvals"
            },
            {
                new Guid("77777777-7777-7777-7777-777777777774"),
                "approvals.reject",
                "Reject approvals"
            },
            {
                new Guid("77777777-7777-7777-7777-777777777775"),
                "approvals.cancel",
                "Cancel approvals"
            }
        });

    migrationBuilder.InsertData(
        table: "RolePermissions",
        columns: new[] { "PermissionId", "RoleId" },
        values: new object[,]
        {
            {
                new Guid("77777777-7777-7777-7777-777777777771"),
                new Guid("11111111-1111-1111-1111-111111111111")
            },
            {
                new Guid("77777777-7777-7777-7777-777777777772"),
                new Guid("11111111-1111-1111-1111-111111111111")
            },
            {
                new Guid("77777777-7777-7777-7777-777777777773"),
                new Guid("11111111-1111-1111-1111-111111111111")
            },
            {
                new Guid("77777777-7777-7777-7777-777777777774"),
                new Guid("11111111-1111-1111-1111-111111111111")
            },
            {
                new Guid("77777777-7777-7777-7777-777777777775"),
                new Guid("11111111-1111-1111-1111-111111111111")
            }
        });
}

        /// <inheritdoc />
 protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DeleteData(
        table: "RolePermissions",
        keyColumns: new[] { "PermissionId", "RoleId" },
        keyValues: new object[]
        {
            new Guid("77777777-7777-7777-7777-777777777771"),
            new Guid("11111111-1111-1111-1111-111111111111")
        });

    migrationBuilder.DeleteData(
        table: "RolePermissions",
        keyColumns: new[] { "PermissionId", "RoleId" },
        keyValues: new object[]
        {
            new Guid("77777777-7777-7777-7777-777777777772"),
            new Guid("11111111-1111-1111-1111-111111111111")
        });

    migrationBuilder.DeleteData(
        table: "RolePermissions",
        keyColumns: new[] { "PermissionId", "RoleId" },
        keyValues: new object[]
        {
            new Guid("77777777-7777-7777-7777-777777777773"),
            new Guid("11111111-1111-1111-1111-111111111111")
        });

    migrationBuilder.DeleteData(
        table: "RolePermissions",
        keyColumns: new[] { "PermissionId", "RoleId" },
        keyValues: new object[]
        {
            new Guid("77777777-7777-7777-7777-777777777774"),
            new Guid("11111111-1111-1111-1111-111111111111")
        });

    migrationBuilder.DeleteData(
        table: "RolePermissions",
        keyColumns: new[] { "PermissionId", "RoleId" },
        keyValues: new object[]
        {
            new Guid("77777777-7777-7777-7777-777777777775"),
            new Guid("11111111-1111-1111-1111-111111111111")
        });

    migrationBuilder.DeleteData(
        table: "Permissions",
        keyColumn: "Id",
        keyValue: new Guid("77777777-7777-7777-7777-777777777771"));

    migrationBuilder.DeleteData(
        table: "Permissions",
        keyColumn: "Id",
        keyValue: new Guid("77777777-7777-7777-7777-777777777772"));

    migrationBuilder.DeleteData(
        table: "Permissions",
        keyColumn: "Id",
        keyValue: new Guid("77777777-7777-7777-7777-777777777773"));

    migrationBuilder.DeleteData(
        table: "Permissions",
        keyColumn: "Id",
        keyValue: new Guid("77777777-7777-7777-7777-777777777774"));

    migrationBuilder.DeleteData(
        table: "Permissions",
        keyColumn: "Id",
        keyValue: new Guid("77777777-7777-7777-7777-777777777775"));
}
    }
}
