using System;
using System.Collections.Generic;
using System.Linq;
using BillTracker.Api.Models;
using BillTracker.Expenses;
using FluentAssertions;
using Xunit;

namespace BillTracker.Tests
{
    public class ModelMappingsTests
    {
        [Fact]
        public void MapExpensesToDashboardResponse()
        {
            var mostExpensive = new ExpenseModel(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, "name1", 15);
            var expenses = new List<ExpenseModel>
            {
                new ExpenseModel(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, "name1", 12),
                new ExpenseModel(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, "name1", 13),
                new ExpenseModel(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, "name1", 14),
                mostExpensive,
            };

            var result = ModelMappings.MapExpensesToDashboardResponse(expenses);

            result.MostExpensive.Should().BeEquivalentTo(mostExpensive);
            result.TotalTransfers.Should().Be(expenses.Count);
            result.Total.Should().Be(expenses.Sum(x => x.Amount));
        }

        [Fact]
        public void MapExpensesToDashboardEmpty()
        {
            var expenses = new List<ExpenseModel>();

            var result = ModelMappings.MapExpensesToDashboardResponse(expenses);

            result.MostExpensive.Should().BeNull();
            result.TotalTransfers.Should().Be(0);
            result.Total.Should().Be(0);
        }
    }
}
