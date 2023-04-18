using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalSalesManagement.Infrastructure.Migrations
{
    public partial class V23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "AgentCommissionApprovals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "AgentCommissionApprovals");
        }
    }
}
