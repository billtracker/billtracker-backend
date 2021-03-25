using BillTracker.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BillTracker.Migrations
{
    public partial class ExpenseTypesSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
INSERT INTO ""ExpenseTypes"" (""Id"", ""Name"", ""IsBuiltIn"") VALUES ('4a7824e3-cd91-4717-9016-ceeef182d9bd', 'Food', 'true');
INSERT INTO ""ExpenseTypes"" (""Id"", ""Name"", ""IsBuiltIn"") VALUES ('6e20249e-dc6e-4b0f-acb0-123c477f882e', 'Entertainment', 'true');
INSERT INTO ""ExpenseTypes"" (""Id"", ""Name"", ""IsBuiltIn"") VALUES ('330d39ce-a2bc-4ff3-8908-31865090230e', 'Gas', 'true');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No need to implement this.
        }
    }
}
