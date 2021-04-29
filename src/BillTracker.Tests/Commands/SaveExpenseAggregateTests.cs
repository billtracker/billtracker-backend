using System;
using System.Threading.Tasks;
using BillTracker.Commands;
using BillTracker.Entities;
using BillTracker.Queries;
using BillTracker.Shared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests.Commands
{
    public class SaveExpenseAggregateTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;
        private readonly ExpenseType TestExpenseType;

        public SaveExpenseAggregateTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
            TestExpenseType = _fixture.CreateExpenseType(TestUser.Id);
        }

        [Fact]
        public async Task WhenAggregateIdIsNullThenCreatesNew()
        {
            var sut = _factory.Services.GetRequiredService<SaveExpenseAggregate>();
            var expensesQuery = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.Handle(new SaveExpenseAggregateParameters(null, TestUser.Id, "name", 20));
            var newAggregate = await expensesQuery.GetExpensesAggregate(result.Result);

            result.IsError.Should().BeFalse();
            newAggregate.IsDraft.Should().BeFalse();
            newAggregate.Expenses.Should().BeEmpty();
            newAggregate.Name.Should().Be("name");
            newAggregate.TotalExpensesPrice.Should().Be(0);
            newAggregate.UserId.Should().Be(TestUser.Id);
        }

        [Fact]
        public async Task WhenAggregateIdIsNotNullAndDoesNotExistThenReturnsError()
        {
            var sut = _factory.Services.GetRequiredService<SaveExpenseAggregate>();

            var result = await sut.Handle(new SaveExpenseAggregateParameters(Guid.NewGuid(), TestUser.Id, "name", 20));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.ExpenseAggregateNotFound);
        }
    }
}
