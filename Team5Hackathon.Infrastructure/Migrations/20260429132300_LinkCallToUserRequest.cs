using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team5Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkCallToUserRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CallId",
                table: "UserRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "UserRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sentiment",
                table: "UserRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Calls",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRequests_CallId",
                table: "UserRequests",
                column: "CallId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRequests_CallId",
                table: "UserRequests");

            migrationBuilder.DropColumn(
                name: "CallId",
                table: "UserRequests");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "UserRequests");

            migrationBuilder.DropColumn(
                name: "Sentiment",
                table: "UserRequests");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Calls");
        }
    }
}
