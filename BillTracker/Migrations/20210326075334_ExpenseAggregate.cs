using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BillTracker.Migrations
{
    public partial class ExpenseAggregate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "Expenses");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Expenses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "AggregateId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpensesAggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AddedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpensesAggregates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpensesAggregates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_AggregateId",
                table: "Expenses",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpensesAggregates_UserId",
                table: "ExpensesAggregates",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpensesAggregates_AggregateId",
                table: "Expenses",
                column: "AggregateId",
                principalTable: "ExpensesAggregates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpensesAggregates_AggregateId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "ExpensesAggregates");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_AggregateId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "AggregateId",
                table: "Expenses");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AddedDate",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
