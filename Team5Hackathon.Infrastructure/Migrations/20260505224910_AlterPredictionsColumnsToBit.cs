using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team5Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterPredictionsColumnsToBit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Predictions",
                table: "Predictions");

            migrationBuilder.RenameTable(
                name: "Predictions",
                newName: "predictions");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "predictions",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_customer_type",
                table: "predictions",
                newName: "IX_predictions_customer_type");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_customer_id",
                table: "predictions",
                newName: "IX_predictions_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Predictions_churn_risk_score",
                table: "predictions",
                newName: "IX_predictions_churn_risk_score");

            migrationBuilder.AlterColumn<double>(
                name: "usage_change_pct",
                table: "predictions",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "timestamp",
                table: "predictions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "recharge_freq",
                table: "predictions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "recharge_change_pct",
                table: "predictions",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "network_quality",
                table: "predictions",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<bool>(
                name: "has_complaint",
                table: "predictions",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "engagement_score",
                table: "predictions",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "customer_type",
                table: "predictions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "predictions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "churn_risk_score",
                table: "predictions",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<bool>(
                name: "churn",
                table: "predictions",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "avg_data_usage",
                table: "predictions",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddPrimaryKey(
                name: "PK_predictions",
                table: "predictions",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_predictions",
                table: "predictions");

            migrationBuilder.RenameTable(
                name: "predictions",
                newName: "Predictions");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Predictions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_predictions_customer_type",
                table: "Predictions",
                newName: "IX_Predictions_customer_type");

            migrationBuilder.RenameIndex(
                name: "IX_predictions_customer_id",
                table: "Predictions",
                newName: "IX_Predictions_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_predictions_churn_risk_score",
                table: "Predictions",
                newName: "IX_Predictions_churn_risk_score");

            migrationBuilder.AlterColumn<double>(
                name: "usage_change_pct",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "timestamp",
                table: "Predictions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "recharge_freq",
                table: "Predictions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "recharge_change_pct",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "network_quality",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "has_complaint",
                table: "Predictions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "engagement_score",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "customer_type",
                table: "Predictions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "Predictions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "churn_risk_score",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "churn",
                table: "Predictions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "avg_data_usage",
                table: "Predictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Predictions",
                table: "Predictions",
                column: "Id");
        }
    }
}
