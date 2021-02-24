using System;

namespace BillTracker.Entities
{
    public static class BuiltInExpenseTypes
    {
        public const int Amount = 3;

        public static readonly ExpenseType Food = new ExpenseType(Guid.Parse("4a7824e3-cd91-4717-9016-ceeef182d9bd"), nameof(Food));
        public static readonly ExpenseType Entertainment = new ExpenseType(Guid.Parse("6e20249e-dc6e-4b0f-acb0-123c477f882e"), nameof(Entertainment));
        public static readonly ExpenseType Gas = new ExpenseType(Guid.Parse("330d39ce-a2bc-4ff3-8908-31865090230e"), nameof(Gas));

        public class ExpenseType
        {
            public ExpenseType(Guid id, string name)
            {
                Id = id;
                Name = name;
            }

            public Guid Id { get; }

            public string Name { get; }
        }
    }
}
