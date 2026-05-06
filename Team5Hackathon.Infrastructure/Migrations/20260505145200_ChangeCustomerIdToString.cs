using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team5Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCustomerIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, we need to drop foreign key references and indexes if they exist

            // Drop indexes on CustomerId columns
            migrationBuilder.DropIndex(
                name: "IX_InterventionQueues_CustomerId",
                table: "InterventionQueues");

            migrationBuilder.DropIndex(
                name: "IX_CustomerRiskProfiles_ChurnRiskScore",
                table: "CustomerRiskProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CustomerRiskProfiles_CustomerType",
                table: "CustomerRiskProfiles");

            // Drop the primary key constraint on CustomerRiskProfiles
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerRiskProfiles",
                table: "CustomerRiskProfiles");

            // Create a temporary column for the new string CustomerId in InterventionQueues
            migrationBuilder.AddColumn<string>(
                name: "CustomerIdTemp",
                table: "InterventionQueues",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Update the temporary column with converted values from the old int column
            migrationBuilder.Sql("UPDATE InterventionQueues SET CustomerIdTemp = CAST(CustomerId AS NVARCHAR(50))");

            // Drop the old CustomerId column
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "InterventionQueues");

            // Rename the temporary column to CustomerId
            migrationBuilder.RenameColumn(
                name: "CustomerIdTemp",
                table: "InterventionQueues",
                newName: "CustomerId");

            // Create a temporary column for the new string CustomerId in CustomerRiskProfiles
            migrationBuilder.AddColumn<string>(
                name: "CustomerIdTemp",
                table: "CustomerRiskProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Update the temporary column with converted values from the old int column
            migrationBuilder.Sql("UPDATE CustomerRiskProfiles SET CustomerIdTemp = CAST(CustomerId AS NVARCHAR(50))");

            // Drop the old CustomerId column
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CustomerRiskProfiles");

            // Rename the temporary column to CustomerId
            migrationBuilder.RenameColumn(
                name: "CustomerIdTemp",
                table: "CustomerRiskProfiles",
                newName: "CustomerId");

            // Recreate the primary key on CustomerRiskProfiles
            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerRiskProfiles",
                table: "CustomerRiskProfiles",
                column: "CustomerId");

            // Recreate the indexes
            migrationBuilder.CreateIndex(
                name: "IX_InterventionQueues_CustomerId",
                table: "InterventionQueues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRiskProfiles_ChurnRiskScore",
                table: "CustomerRiskProfiles",
                column: "ChurnRiskScore");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRiskProfiles_CustomerType",
                table: "CustomerRiskProfiles",
                column: "CustomerType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes first
            migrationBuilder.DropIndex(
                name: "IX_InterventionQueues_CustomerId",
                table: "InterventionQueues");

            migrationBuilder.DropIndex(
                name: "IX_CustomerRiskProfiles_ChurnRiskScore",
                table: "CustomerRiskProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CustomerRiskProfiles_CustomerType",
                table: "CustomerRiskProfiles");

            // Drop primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerRiskProfiles",
                table: "CustomerRiskProfiles");

            // Handle InterventionQueues table - convert back to int
            migrationBuilder.AddColumn<int>(
                name: "CustomerIdTemp",
                table: "InterventionQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Update with converted values (this assumes all string values can be converted to int)
            migrationBuilder.Sql("UPDATE InterventionQueues SET CustomerIdTemp = CAST(CustomerId AS INT) WHERE ISNUMERIC(CustomerId) = 1");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "InterventionQueues");

            migrationBuilder.RenameColumn(
                name: "CustomerIdTemp",
                table: "InterventionQueues",
                newName: "CustomerId");

            // Handle CustomerRiskProfiles table - convert back to int with IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "CustomerIdTemp",
                table: "CustomerRiskProfiles",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            // Note: We can't perfectly restore the original int values due to the IDENTITY constraint
            // This is a limitation when reverting this type of change
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CustomerRiskProfiles");

            migrationBuilder.RenameColumn(
                name: "CustomerIdTemp",
                table: "CustomerRiskProfiles",
                newName: "CustomerId");

            // Recreate primary key and indexes
            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerRiskProfiles",
                table: "CustomerRiskProfiles",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionQueues_CustomerId",
                table: "InterventionQueues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRiskProfiles_ChurnRiskScore",
                table: "CustomerRiskProfiles",
                column: "ChurnRiskScore");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRiskProfiles_CustomerType",
                table: "CustomerRiskProfiles",
                column: "CustomerType");
        }
    }
}