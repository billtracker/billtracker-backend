using BillTracker.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BillTracker.Migrations
{
    public partial class ExpenseTypesSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                {MigrationHelpers.InsertBuiltInExpenseType(BuiltInExpenseTypes.Food)}
                {MigrationHelpers.InsertBuiltInExpenseType(BuiltInExpenseTypes.Entertainment)}
                {MigrationHelpers.InsertBuiltInExpenseType(BuiltInExpenseTypes.Gas)}
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No need to implement this.
        }
    }
}
