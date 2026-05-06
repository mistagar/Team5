using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team5Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class predictiontables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CooldownMinutes = table.Column<int>(type: "int", nullable: false),
                    PreferredChurnTypes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerRiskProfiles",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvgDataUsage = table.Column<double>(type: "float", nullable: false),
                    UsageChangePct = table.Column<double>(type: "float", nullable: false),
                    RechargeFreq = table.Column<int>(type: "int", nullable: false),
                    RechargeChangePct = table.Column<double>(type: "float", nullable: false),
                    EngagementScore = table.Column<double>(type: "float", nullable: false),
                    NetworkQuality = table.Column<double>(type: "float", nullable: false),
                    HasComplaint = table.Column<bool>(type: "bit", nullable: false),
                    ComplaintText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Churn = table.Column<bool>(type: "bit", nullable: false),
                    ChurnRiskScore = table.Column<double>(type: "float", nullable: false),
                    CustomerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRiskProfiles", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RiskThreshold = table.Column<double>(type: "float", nullable: false),
                    ApplicableSegments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterventionQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    RecommendedOffer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecommendedChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedOffer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedChannel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionQueues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfferConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ValidityStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidityEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OfferType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Terms = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfferEligibilityRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChurnClassification = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MinChurnRisk = table.Column<double>(type: "float", nullable: true),
                    MaxChurnRisk = table.Column<double>(type: "float", nullable: true),
                    MinEngagement = table.Column<double>(type: "float", nullable: true),
                    MaxEngagement = table.Column<double>(type: "float", nullable: true),
                    HasComplaint = table.Column<bool>(type: "bit", nullable: true),
                    ComplaintText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinNetworkQuality = table.Column<double>(type: "float", nullable: true),
                    MaxNetworkQuality = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferEligibilityRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferEligibilityRules_OfferConfigurations_OfferConfigurationId",
                        column: x => x.OfferConfigurationId,
                        principalTable: "OfferConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelConfigurations_ChannelType",
                table: "ChannelConfigurations",
                column: "ChannelType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelConfigurations_IsEnabled",
                table: "ChannelConfigurations",
                column: "IsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRiskProfiles_ChurnRiskScore",
                table: "CustomerRiskProfiles",
                column: "ChurnRiskScore");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRiskProfiles_CustomerType",
                table: "CustomerRiskProfiles",
                column: "CustomerType");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionPolicies_IsActive",
                table: "ExecutionPolicies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionQueues_CustomerId",
                table: "InterventionQueues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionQueues_Status",
                table: "InterventionQueues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OfferConfigurations_IsActive",
                table: "OfferConfigurations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OfferConfigurations_ValidityEnd",
                table: "OfferConfigurations",
                column: "ValidityEnd");

            migrationBuilder.CreateIndex(
                name: "IX_OfferConfigurations_ValidityStart",
                table: "OfferConfigurations",
                column: "ValidityStart");

            migrationBuilder.CreateIndex(
                name: "IX_OfferEligibilityRules_OfferConfigurationId",
                table: "OfferEligibilityRules",
                column: "OfferConfigurationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelConfigurations");

            migrationBuilder.DropTable(
                name: "CustomerRiskProfiles");

            migrationBuilder.DropTable(
                name: "ExecutionPolicies");

            migrationBuilder.DropTable(
                name: "InterventionQueues");

            migrationBuilder.DropTable(
                name: "OfferEligibilityRules");

            migrationBuilder.DropTable(
                name: "OfferConfigurations");
        }
    }
}
