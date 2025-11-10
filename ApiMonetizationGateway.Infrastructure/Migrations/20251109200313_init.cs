using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiMonetizationGateway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tiers",
                columns: table => new
                {
                    TierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyQuota = table.Column<int>(type: "int", nullable: false),
                    RateLimit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiers", x => x.TierId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TierId = table.Column<int>(type: "int", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Tiers_TierId",
                        column: x => x.TierId,
                        principalTable: "Tiers",
                        principalColumn: "TierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiUsageLogs",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiUsageLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_ApiUsageLogs_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyUsageSummary",
                columns: table => new
                {
                    SummaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    MonthYear = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalRequests = table.Column<int>(type: "int", nullable: false),
                    AmountBilled = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyUsageSummary", x => x.SummaryId);
                    table.ForeignKey(
                        name: "FK_MonthlyUsageSummary_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tiers",
                columns: new[] { "TierId", "MonthlyQuota", "Name", "Price", "RateLimit" },
                values: new object[,]
                {
                    { 1, 100, "Free", 0m, 2 },
                    { 2, 100000, "Pro", 50.00m, 10 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "ApiKey", "CreatedAt", "Email", "IsActive", "Name", "TierId", "UserId" },
                values: new object[,]
                {
                    { 1, "asftdtyfqwy2332jb423ui4b3u2b324", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "umair@gmail.com", true, "Umair", 1, new Guid("5cd25fa8-b3b3-4926-96b3-aa111c9fee75") },
                    { 2, "qwftwefqwy2332jb423ui4b3u2b334", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ali@gmail.com", true, "Ali", 2, new Guid("6cd23fa8-b3b3-4926-96b3-aa111c9fee55") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_CustomerId_Timestamp",
                table: "ApiUsageLogs",
                columns: new[] { "CustomerId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_Timestamp",
                table: "ApiUsageLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ApiKey",
                table: "Customers",
                column: "ApiKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TierId",
                table: "Customers",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyUsageSummary_CustomerId",
                table: "MonthlyUsageSummary",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiUsageLogs");

            migrationBuilder.DropTable(
                name: "MonthlyUsageSummary");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Tiers");
        }
    }
}
