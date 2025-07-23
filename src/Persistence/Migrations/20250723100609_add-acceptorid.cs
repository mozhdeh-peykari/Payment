using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addacceptorid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalServiceResponseCode",
                table: "TransactionEvents");

            migrationBuilder.RenameColumn(
                name: "RequestTime",
                table: "PaymentTransactions",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "ConfirmedTime",
                table: "PaymentTransactions",
                newName: "ConfirmedDate");

            migrationBuilder.AddColumn<string>(
                name: "AcceptorId",
                table: "PaymentTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "PaymentTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptorId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "PaymentTransactions",
                newName: "RequestTime");

            migrationBuilder.RenameColumn(
                name: "ConfirmedDate",
                table: "PaymentTransactions",
                newName: "ConfirmedTime");

            migrationBuilder.AddColumn<string>(
                name: "ExternalServiceResponseCode",
                table: "TransactionEvents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
