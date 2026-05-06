using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team5Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatePredictionsTableFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "predictions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: true),
                    avg_data_usage = table.Column<double>(type: "float", nullable: true),
                    usage_change_pct = table.Column<double>(type: "float", nullable: true),
                    recharge_freq = table.Column<int>(type: "int", nullable: true),
                    recharge_change_pct = table.Column<double>(type: "float", nullable: true),
                    engagement_score = table.Column<double>(type: "float", nullable: true),
                    network_quality = table.Column<double>(type: "float", nullable: true),
                    has_complaint = table.Column<bool>(type: "bit", nullable: true),
                    complaint_text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    churn = table.Column<bool>(type: "bit", nullable: true),
                    churn_risk_score = table.Column<double>(type: "float", nullable: true),
                    customer_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_predictions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_predictions_churn_risk_score",
                table: "predictions",
                column: "churn_risk_score");

            migrationBuilder.CreateIndex(
                name: "IX_predictions_customer_id",
                table: "predictions",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_predictions_customer_type",
                table: "predictions",
                column: "customer_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "predictions");
        }
    }
}
