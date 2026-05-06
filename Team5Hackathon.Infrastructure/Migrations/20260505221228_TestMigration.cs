using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team5Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Predictions",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Churn",
                table: "Predictions",
                newName: "churn");

            migrationBuilder.RenameColumn(
                name: "UsageChangePct",
                table: "Predictions",
                newName: "usage_change_pct");

            migrationBuilder.RenameColumn(
                name: "RechargeFreq",
                table: "Predictions",
                newName: "recharge_freq");

            migrationBuilder.RenameColumn(
                name: "RechargeChangePct",
                table: "Predictions",
                newName: "recharge_change_pct");

            migrationBuilder.RenameColumn(
                name: "NetworkQuality",
                table: "Predictions",
                newName: "network_quality");

            migrationBuilder.RenameColumn(
                name: "HasComplaint",
                table: "Predictions",
                newName: "has_complaint");

            migrationBuilder.RenameColumn(
                name: "EngagementScore",
                table: "Predictions",
                newName: "engagement_score");

            migrationBuilder.RenameColumn(
                name: "CustomerType",
                table: "Predictions",
                newName: "customer_type");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Predictions",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "ComplaintText",
                table: "Predictions",
                newName: "complaint_text");

            migrationBuilder.RenameColumn(
                name: "ChurnRiskScore",
                table: "Predictions",
                newName: "churn_risk_score");

            migrationBuilder.RenameColumn(
                name: "AvgDataUsage",
                table: "Predictions",
                newName: "avg_data_usage");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_CustomerType",
                table: "Predictions",
                newName: "IX_Predictions_customer_type");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_CustomerId",
                table: "Predictions",
                newName: "IX_Predictions_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_ChurnRiskScore",
                table: "Predictions",
                newName: "IX_Predictions_churn_risk_score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "Predictions",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "churn",
                table: "Predictions",
                newName: "Churn");

            migrationBuilder.RenameColumn(
                name: "usage_change_pct",
                table: "Predictions",
                newName: "UsageChangePct");

            migrationBuilder.RenameColumn(
                name: "recharge_freq",
                table: "Predictions",
                newName: "RechargeFreq");

            migrationBuilder.RenameColumn(
                name: "recharge_change_pct",
                table: "Predictions",
                newName: "RechargeChangePct");

            migrationBuilder.RenameColumn(
                name: "network_quality",
                table: "Predictions",
                newName: "NetworkQuality");

            migrationBuilder.RenameColumn(
                name: "has_complaint",
                table: "Predictions",
                newName: "HasComplaint");

            migrationBuilder.RenameColumn(
                name: "engagement_score",
                table: "Predictions",
                newName: "EngagementScore");

            migrationBuilder.RenameColumn(
                name: "customer_type",
                table: "Predictions",
                newName: "CustomerType");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "Predictions",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "complaint_text",
                table: "Predictions",
                newName: "ComplaintText");

            migrationBuilder.RenameColumn(
                name: "churn_risk_score",
                table: "Predictions",
                newName: "ChurnRiskScore");

            migrationBuilder.RenameColumn(
                name: "avg_data_usage",
                table: "Predictions",
                newName: "AvgDataUsage");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_customer_type",
                table: "Predictions",
                newName: "IX_Predictions_CustomerType");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_customer_id",
                table: "Predictions",
                newName: "IX_Predictions_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_churn_risk_score",
                table: "Predictions",
                newName: "IX_Predictions_ChurnRiskScore");
        }
    }
}
