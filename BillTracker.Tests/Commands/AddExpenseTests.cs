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
    public class AddExpenseTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;
        private readonly ExpenseType TestExpenseType;

        public AddExpenseTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
            TestExpenseType = _fixture.CreateExpenseType(TestUser.Id);
        }

        [Fact]
        public async Task UserWhoNotExistCannotAddExpense()
        {
            var sut = _factory.Services.GetRequiredService<AddExpense>();

            var result = await sut.Handle(new AddExpenseParameters(Guid.NewGuid(), "name", 20, Guid.NewGuid()));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.UserNotExist);
        }

        [Fact]
        public async Task CannotAddExpenseWhenTypeDoesNotExist()
        {
            var sut = _factory.Services.GetRequiredService<AddExpense>();

            var result = await sut.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, Guid.NewGuid()));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(AddExpense.ExpenseTypeNotExist);
        }

        [Fact]
        public async Task UserCanAddExpenseWithCustomType()
        {
            var sut = _factory.Services.GetRequiredService<AddExpense>();
            var query = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType.Id));
            var addedExpense = await query.GetById(result.Result);

            result.IsError.Should().BeFalse();
            result.Result.Should().NotBeEmpty();
            addedExpense.UserId.Should().Be(TestUser.Id);
            addedExpense.ExpenseTypeName.Should().Be(TestExpenseType.Name);
            addedExpense.Amount.Should().Be(20);
        }

        [Fact]
        public async Task AddingExpenseWithoutAggregateIdCreatesNewAggregateWithSameNameAsExpense()
        {
            var sut = _factory.Services.GetRequiredService<AddExpense>();
            var query = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType.Id));
            var addedExpense = await query.GetById(result.Result);

            result.IsError.Should().BeFalse();
            result.Result.Should().NotBeEmpty();
            addedExpense.AggregateId.Should().NotBeEmpty();
            addedExpense.AggregateName.Should().Be(addedExpense.Name);
        }

        [Fact]
        public async Task AddingExpenseWithNoExistingAggregateReturnsError()
        {
            var sut = _factory.Services.GetRequiredService<AddExpense>();

            var result = await sut.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType.Id, aggregateId: Guid.NewGuid()));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.ExpenseAggregateDoesNotExist);
        }
    }
}
