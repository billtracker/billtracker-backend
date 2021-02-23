using BillTracker.Entities;

namespace BillTracker.Migrations
{
    internal static class MigrationHelpers
    {
        public static string InsertBuiltInExpenseType(BuiltInExpenseTypes.ExpenseType type)
            => $"INSERT INTO \"ExpenseTypes\" (\"Id\", \"Name\", \"IsBuiltIn\") VALUES ('{type.Id}', '{type.Name}', 'true');";
    }
}
