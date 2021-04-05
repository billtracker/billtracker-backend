using System;

namespace BillTracker.Models
{
    public record ExpenseTypeModel
    {
        public Guid Id { get; init; }

        public Guid? UserId { get; init; }

        public string Name { get; init; }
    }
}
