using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalSalesManagement.Infrastructure.Migrations
{
    public partial class v55 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "AgentCommissionApprovals",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "AgentCommissionApprovals");
        }
    }
}
