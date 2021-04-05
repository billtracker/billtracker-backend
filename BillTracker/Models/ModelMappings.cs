using System.Linq;
using BillTracker.Entities;

namespace BillTracker.Models
{
    internal static class ModelMappings
    {
        public static ExpenseAggregateModel ToModel(this ExpensesAggregate entity) =>
               new ExpenseAggregateModel
               {
                   Id = entity.Id,
                   Name = entity.Name,
                   UserId = entity.UserId,
                   IsDraft = entity.IsDraft,
                   Expenses = entity.Expenses.Select(x => x.ToModel()),
                   Bills = entity.ExpenseBillFiles.Select(x => x.ToModel()),
               };

        public static ExpenseModel ToModel(this Expense expense) =>
            new ExpenseModel
            {
                Id = expense.Id,
                Name = expense.Name,
                Amount = expense.Amount,
                ExpenseTypeId = expense.ExpenseTypeId,
                AggregateId = expense.AggregateId,
            };

        public static ExpenseTypeModel ToModel(this ExpenseType expenseType) =>
            new ExpenseTypeModel
            {
                Id = expenseType.Id,
                UserId = expenseType.UserId,
                Name = expenseType.Name,
            };

        public static ExpenseBillFileModel ToModel(this ExpenseBillFile entity) =>
            new ExpenseBillFileModel
            {
                Id = entity.Id,
                AggregateId = entity.AggregateId,
                FileName = entity.FileName,
                FileUri = entity.FileUri,
            };
    }
}
