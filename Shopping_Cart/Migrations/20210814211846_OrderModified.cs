using Microsoft.EntityFrameworkCore.Migrations;

namespace Shopping_Cart.Migrations
{
    public partial class OrderModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderDetailtId",
                table: "OrderDetails",
                newName: "OrderDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "OrderDetails",
                newName: "OrderDetailtId");
        }
    }
}
