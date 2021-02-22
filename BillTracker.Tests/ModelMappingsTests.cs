using System;
using System.Collections.Generic;
using System.Linq;
using BillTracker.Api.Models;
using BillTracker.Models;
using FluentAssertions;
using Xunit;

namespace BillTracker.Tests
{
    public class ModelMappingsTests
    {
        //[Fact]
        //public void MapExpensesToDashboardResponse()
        //{
        //    var mostExpensive = new ExpenseModel(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, "name1", 15);
        //    var expenses = new List<ExpenseModel>
        //    {
        //        new ExpenseModel(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, "name1", 12),
        //        new ExpenseModel(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, "name1", 13),
        //        new ExpenseModel(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now, "name1", 14),
        //        mostExpensive,
        //    };

        //    var result = DashboardResponseFactory.Create(expenses);

        //    result.Metrics.MostExpensive.Should().BeEquivalentTo(mostExpensive);
        //    result.Metrics.TotalTransfers.Should().Be(expenses.Count);
        //    result.Metrics.Total.Should().Be(expenses.Sum(x => x.Amount));
        //}

        //[Fact]
        //public void MapExpensesToDashboardEmpty()
        //{
        //    var expenses = new List<ExpenseModel>();

        //    var result = DashboardResponseFactory.Create(expenses);

        //    result.Metrics.MostExpensive.Should().BeNull();
        //    result.Metrics.TotalTransfers.Should().Be(0);
        //    result.Metrics.Total.Should().Be(0);
        //}
    }
}
