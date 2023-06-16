using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomaticRecurringPayments.Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductAccessProviderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductAccessProviderId",
                table: "BraintreeTransactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductAccessProviderId",
                table: "BraintreeTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
