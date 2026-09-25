using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpsFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeSystemRoleNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Roles
                SET NormalizedName = CASE Id
                    WHEN '11111111-1111-1111-1111-111111111111' THEN 'OWNER'
                    WHEN '22222222-2222-2222-2222-222222222222' THEN 'ADMIN'
                    WHEN '33333333-3333-3333-3333-333333333333' THEN 'PROJECT_MANAGER'
                    WHEN '44444444-4444-4444-4444-444444444444' THEN 'TEAM_LEAD'
                    WHEN '55555555-5555-5555-5555-555555555555' THEN 'MEMBER'
                    WHEN '66666666-6666-6666-6666-666666666666' THEN 'VIEWER'
                END
                WHERE Id IN (
                    '11111111-1111-1111-1111-111111111111',
                    '22222222-2222-2222-2222-222222222222',
                    '33333333-3333-3333-3333-333333333333',
                    '44444444-4444-4444-4444-444444444444',
                    '55555555-5555-5555-5555-555555555555',
                    '66666666-6666-6666-6666-666666666666'
                );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Roles
                SET NormalizedName = CASE Id
                    WHEN '11111111-1111-1111-1111-111111111111' THEN 'OWNER1'
                    WHEN '22222222-2222-2222-2222-222222222222' THEN 'ADMIN1'
                    WHEN '33333333-3333-3333-3333-333333333333' THEN 'PROJECT_MANAGER1'
                    WHEN '44444444-4444-4444-4444-444444444444' THEN 'TEAM_LEAD1'
                    WHEN '55555555-5555-5555-5555-555555555555' THEN 'MEMBER1'
                    WHEN '66666666-6666-6666-6666-666666666666' THEN 'VIEWER1'
                END
                WHERE Id IN (
                    '11111111-1111-1111-1111-111111111111',
                    '22222222-2222-2222-2222-222222222222',
                    '33333333-3333-3333-3333-333333333333',
                    '44444444-4444-4444-4444-444444444444',
                    '55555555-5555-5555-5555-555555555555',
                    '66666666-6666-6666-6666-666666666666'
                );
            """);
        }
    }
}
