using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BillTracker.Migrations
{
    public partial class ExpenseAggregateIdIsRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpensesAggregates_AggregateId",
                table: "Expenses");

            migrationBuilder.AlterColumn<Guid>(
                name: "AggregateId",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpensesAggregates_AggregateId",
                table: "Expenses",
                column: "AggregateId",
                principalTable: "ExpensesAggregates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpensesAggregates_AggregateId",
                table: "Expenses");

            migrationBuilder.AlterColumn<Guid>(
                name: "AggregateId",
                table: "Expenses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpensesAggregates_AggregateId",
                table: "Expenses",
                column: "AggregateId",
                principalTable: "ExpensesAggregates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
