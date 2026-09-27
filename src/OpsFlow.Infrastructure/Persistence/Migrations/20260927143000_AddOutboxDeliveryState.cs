using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OpsFlow.Infrastructure.Persistence;

#nullable disable

namespace OpsFlow.Infrastructure.Persistence.Migrations;

[DbContext(typeof(OpsFlowDbContext))]
[Migration("20260927143000_AddOutboxDeliveryState")]
public sealed class AddOutboxDeliveryState : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_OutboxMessages_ProcessedAt_OccurredAt",
            table: "OutboxMessages");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "DeadLetteredAt",
            table: "OutboxMessages",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "DeliveryAttempts",
            table: "OutboxMessages",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "NextAttemptAt",
            table: "OutboxMessages",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_DeadLetteredAt_ProcessedAt_NextAttemptAt_OccurredAt",
            table: "OutboxMessages",
            columns: new[]
            {
                "DeadLetteredAt",
                "ProcessedAt",
                "NextAttemptAt",
                "OccurredAt"
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_OutboxMessages_DeadLetteredAt_ProcessedAt_NextAttemptAt_OccurredAt",
            table: "OutboxMessages");

        migrationBuilder.DropColumn("DeadLetteredAt", "OutboxMessages");
        migrationBuilder.DropColumn("DeliveryAttempts", "OutboxMessages");
        migrationBuilder.DropColumn("NextAttemptAt", "OutboxMessages");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_ProcessedAt_OccurredAt",
            table: "OutboxMessages",
            columns: new[] { "ProcessedAt", "OccurredAt" });
    }
}
