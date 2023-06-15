using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomaticRecurringPayments.Core.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsumerCode = table.Column<int>(type: "int", nullable: false),
                    BraintreeCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    InsertedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BraintreeTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BraintreeCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethodToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    BraintreeTransactionStatusId = table.Column<int>(type: "int", nullable: true),
                    ProductAccessProviderId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ParentBraintreeTransactionId = table.Column<int>(type: "int", nullable: true),
                    RecurringCanceled = table.Column<bool>(type: "bit", nullable: true),
                    OnDemandStatus = table.Column<int>(type: "int", nullable: true),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    InsertDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    InsertedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BraintreeTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BraintreeTransactions_BraintreeTransactions_ParentBraintreeTransactionId",
                        column: x => x.ParentBraintreeTransactionId,
                        principalTable: "BraintreeTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BraintreeTransactions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    LastBraintreeTransactionId = table.Column<int>(type: "int", nullable: true),
                    InsertDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    InsertedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_BraintreeTransactions_LastBraintreeTransactionId",
                        column: x => x.LastBraintreeTransactionId,
                        principalTable: "BraintreeTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BraintreeTransactions_ClientId",
                table: "BraintreeTransactions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BraintreeTransactions_ParentBraintreeTransactionId",
                table: "BraintreeTransactions",
                column: "ParentBraintreeTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BraintreeTransactions_SubscriptionId",
                table: "BraintreeTransactions",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ClientId",
                table: "Subscriptions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_LastBraintreeTransactionId",
                table: "Subscriptions",
                column: "LastBraintreeTransactionId",
                unique: true,
                filter: "[LastBraintreeTransactionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BraintreeTransactions_Subscriptions_SubscriptionId",
                table: "BraintreeTransactions",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BraintreeTransactions_Clients_ClientId",
                table: "BraintreeTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Clients_ClientId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_BraintreeTransactions_Subscriptions_SubscriptionId",
                table: "BraintreeTransactions");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "BraintreeTransactions");
        }
    }
}
