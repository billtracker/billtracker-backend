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
    public class SaveExpenseTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;
        private readonly ExpenseType TestExpenseType;

        public SaveExpenseTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
            TestExpenseType = _fixture.CreateExpenseType(TestUser.Id);
        }

        [Fact]
        public async Task UserWhoNotExistCannotSaveExpense()
        {
            var sut = _factory.Services.GetRequiredService<SaveExpense>();

            var result = await sut.Handle(new SaveExpenseParameters(Guid.NewGuid(), Guid.NewGuid(), "name", price: 20, amount: 1));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.UserNotExist);
        }

        [Fact]
        public async Task CannotSaveExpenseWhenTypeDoesNotExist()
        {
            var sut = _factory.Services.GetRequiredService<SaveExpense>();

            var result = await sut.Handle(new SaveExpenseParameters(TestUser.Id, Guid.NewGuid(), "name", price: 20, amount: 1, expenseTypeId: Guid.NewGuid()));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(SaveExpense.ExpenseTypeNotFound);
        }

        [Fact]
        public async Task UserCanSaveExpenseWithCustomType()
        {
            var aggId = _fixture.CreateExpenseAggregate(TestUser.Id).Id;
            var sut = _factory.Services.GetRequiredService<SaveExpense>();
            var query = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", price: 20, amount: 1, expenseTypeId: TestExpenseType.Id));
            var addedExpense = await query.GetById(result.Result.Id);

            result.IsError.Should().BeFalse();
            addedExpense.Name.Should().Be("name");
            addedExpense.ExpenseTypeId.Should().Be(TestExpenseType.Id);
            addedExpense.Price.Should().Be(20);
            addedExpense.Amount.Should().Be(1);
        }

        [Fact]
        public async Task UserCanSaveExpenseWithoutType()
        {
            var aggId = _fixture.CreateExpenseAggregate(TestUser.Id).Id;
            var sut = _factory.Services.GetRequiredService<SaveExpense>();
            var query = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", price: 20, amount: 1));
            var addedExpense = await query.GetById(result.Result.Id);

            result.IsError.Should().BeFalse();
            addedExpense.Name.Should().Be("name");
            addedExpense.ExpenseTypeId.Should().BeNull();
            addedExpense.Price.Should().Be(20);
            addedExpense.Amount.Should().Be(1);
        }

        [Fact]
        public async Task SavingExpenseWithNoExistingAggregateReturnsError()
        {
            var sut = _factory.Services.GetRequiredService<SaveExpense>();

            var result = await sut.Handle(new SaveExpenseParameters(TestUser.Id, Guid.NewGuid(), "name", price: 20, amount: 1, expenseTypeId: TestExpenseType.Id));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.ExpenseAggregateNotFound);
        }

        [Fact]
        public async Task SavingExpenseWithExistingAggregateCreatesExpenseWithThatReferencedAggregate()
        {
            var queryExpense = _factory.Services.GetRequiredService<ExpensesQuery>();
            var aggId = _fixture.CreateExpenseAggregate(TestUser.Id).Id;
            var sut = _factory.Services.GetRequiredService<SaveExpense>();

            var result = await sut.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", price: 20, amount: 1, expenseTypeId: TestExpenseType.Id));
            var addedExpense = await queryExpense.GetById(result.Result.Id);

            result.IsError.Should().BeFalse();
            addedExpense.Should().NotBeNull();
            addedExpense.AggregateId.Should().Be(aggId);
        }

        [Fact]
        public async Task SavingExpenseWithNonExistingExpenseIdGetsNull()
        {
            var aggId = _fixture.CreateExpenseAggregate(TestUser.Id).Id;
            var sut = _factory.Services.GetRequiredService<SaveExpense>();

            var result = await sut.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", price: 20, amount: 1, expenseTypeId: TestExpenseType.Id, expenseId: Guid.NewGuid()));
            result.IsError.Should().BeFalse();
            result.Result.Should().BeNull();
        }

        [Fact]
        public async Task SavingExpenseWithExistingExpenseIdUpdatesIt()
        {
            var aggId = _fixture.CreateExpenseAggregate(TestUser.Id).Id;
            var queryExpense = _factory.Services.GetRequiredService<ExpensesQuery>();
            var sut = _factory.Services.GetRequiredService<SaveExpense>();

            var addResult = await sut.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", price: 20, amount: 1, expenseTypeId: TestExpenseType.Id));
            addResult.IsError.Should().BeFalse();
            var updateResult = await sut.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", price: 20, amount: 2, expenseTypeId: TestExpenseType.Id, expenseId: addResult.Result.Id));
            updateResult.IsError.Should().BeFalse();
            var expenseFromDb = await queryExpense.GetById(addResult.Result.Id);

            expenseFromDb.Should().NotBeNull();
            expenseFromDb.Amount.Should().Be(2);
        }
    }
}
