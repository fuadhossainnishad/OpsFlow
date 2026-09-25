using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpsFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DurationSeconds = table.Column<long>(type: "bigint", nullable: true),
                    IsManual = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
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
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000021"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000022"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000023"), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("66666666-6666-6666-6666-666666666666") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_Organization_Project_Started",
                table: "TimeEntries",
                columns: new[] { "OrganizationId", "ProjectId", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_Organization_Task_Started",
                table: "TimeEntries",
                columns: new[] { "OrganizationId", "TaskId", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_Organization_User_Started",
                table: "TimeEntries",
                columns: new[] { "OrganizationId", "UserId", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_ProjectId",
                table: "TimeEntries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_TaskId",
                table: "TimeEntries",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_UserId",
                table: "TimeEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_TimeEntries_Organization_User_Running",
                table: "TimeEntries",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true,
                filter: "[EndedAtUtc] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeEntries");

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
                keyValues: new object[] { new Guid("71000000-0000-0000-0000-000000000020"), new Guid("66666666-6666-6666-6666-666666666666") });

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
        }
    }
}
