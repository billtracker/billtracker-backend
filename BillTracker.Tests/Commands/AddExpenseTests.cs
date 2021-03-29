using System;
using System.Threading.Tasks;
using BillTracker.Commands;
using BillTracker.Entities;
using BillTracker.Queries;
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
            var addedExpense = await query.GetById(result.Result.Id);

            result.IsError.Should().BeFalse();
            addedExpense.Name.Should().Be("name");
            addedExpense.ExpenseTypeId.Should().Be(TestExpenseType.Id);
            addedExpense.Amount.Should().Be(20);
        }

        [Fact]
        public async Task AddingExpenseWithoutAggregateIdCreatesNewAggregateWithSameNameAsExpense()
        {
            var sut = _factory.Services.GetRequiredService<AddExpense>();
            var query = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType.Id));
            var addedExpense = await query.GetById(result.Result.Id);
            var aggregate = await query.GetExpensesAggregate(addedExpense.AggregateId);

            result.IsError.Should().BeFalse();
            addedExpense.AggregateId.Should().NotBeEmpty();
            aggregate.Name.Should().Be("name");
        }

        [Fact]
        public async Task AddingExpenseWithNoExistingAggregateReturnsError()
        {
            var sut = _factory.Services.GetRequiredService<AddExpense>();

            var result = await sut.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType.Id, aggregateId: Guid.NewGuid()));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.ExpenseAggregateNotFound);
        }

        [Fact]
        public async Task AddingExpenseWithExistingAggregateCreatesExpenseWithThatReferencedAggregate()
        {
            var saveAggregate = _factory.Services.GetRequiredService<SaveExpenseAggregate>();
            var queryExpense = _factory.Services.GetRequiredService<ExpensesQuery>();
            var aggregate = await saveAggregate.Handle(new SaveExpenseAggregateParameters(null, TestUser.Id, "agg"));
            var sut = _factory.Services.GetRequiredService<AddExpense>();

            var result = await sut.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType.Id, aggregateId: aggregate.Result));
            var addedExpense = await queryExpense.GetById(result.Result.Id);

            result.IsError.Should().BeFalse();
            addedExpense.Should().NotBeNull();
            addedExpense.AggregateId.Should().Be(aggregate.Result);
        }
    }
}
