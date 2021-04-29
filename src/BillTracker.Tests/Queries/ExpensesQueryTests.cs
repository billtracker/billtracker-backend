using System;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Commands;
using BillTracker.Entities;
using BillTracker.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests.Queries
{
    public class ExpensesQueryTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;
        private readonly ExpenseType TestExpenseType;
        private readonly Guid TestAggregateId;

        public ExpensesQueryTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
            TestExpenseType = _fixture.CreateExpenseType(TestUser.Id);
            TestAggregateId = _fixture.CreateExpenseAggregate(TestUser.Id).Id;
        }

        [Fact]
        public async Task WhenGetMany_GivenNonExistingUser_ThenError()
        {
            var sut = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.GetMany(Guid.NewGuid(), 1);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.UserNotExist);
        }

        [Fact]
        public async Task WhenGetMany_GivenFilters_ThenReturnsPartOfData()
        {
            var initDate = DateTimeOffset.Now;
            var aggId1 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: initDate).Id;
            var aggId2 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: initDate.AddDays(1)).Id;
            var aggId3 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: initDate.AddDays(2)).Id;
            var aggId4 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: initDate.AddDays(3)).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            var expense1 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 15, expenseTypeId: TestExpenseType.Id));
            var expense2 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 16, expenseTypeId: TestExpenseType.Id));
            var expense3 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId3, "name", 17, expenseTypeId: TestExpenseType.Id));
            var expense4 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId4, "name", 18, expenseTypeId: TestExpenseType.Id));
            var sut = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.GetMany(TestUser.Id, 1, fromDate: initDate, toDate: initDate.AddDays(2));

            result.IsError.Should().BeFalse();
            result.Result.TotalItems.Should().Be(3);
            result.Result.Items.Count().Should().Be(3);
            result.Result.Items.Should().Contain(x => x.Id == expense1.Result.Id);
            result.Result.Items.Should().Contain(x => x.Id == expense2.Result.Id);
            result.Result.Items.Should().Contain(x => x.Id == expense3.Result.Id);
            result.Result.Items.Should().NotContain(x => x.Id == expense4.Result.Id);
        }

        [Fact]
        public async Task WhenGetMany_GivenNoFilters_ThenReturnsAllExpenses()
        {
            var aggId1 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now).Id;
            var aggId2 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(1)).Id;
            var aggId3 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(2)).Id;
            var aggId4 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(3)).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            var expense1 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 15, expenseTypeId: TestExpenseType.Id));
            var expense2 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 16, expenseTypeId: TestExpenseType.Id));
            var expense3 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId3, "name", 17, expenseTypeId: TestExpenseType.Id));
            var expense4 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId4, "name", 18, expenseTypeId: TestExpenseType.Id));
            var sut = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.GetMany(TestUser.Id, 1);

            result.IsError.Should().BeFalse();
            result.Result.TotalItems.Should().Be(4);
            result.Result.Items.Count().Should().Be(4);
            result.Result.Items.Should().Contain(x => x.Id == expense1.Result.Id);
            result.Result.Items.Should().Contain(x => x.Id == expense2.Result.Id);
            result.Result.Items.Should().Contain(x => x.Id == expense3.Result.Id);
            result.Result.Items.Should().Contain(x => x.Id == expense4.Result.Id);
        }

        [Fact]
        public async Task WhenGetMany_GivenPaging_ThenReturnsSortedPartOfData()
        {
            var aggId1 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now).Id;
            var aggId2 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(1)).Id;
            var aggId3 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(2)).Id;
            var aggId4 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(3)).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            var expense1 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 15, expenseTypeId: TestExpenseType.Id));
            var expense2 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 16, expenseTypeId: TestExpenseType.Id));
            var expense3 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId3, "name", 17, expenseTypeId: TestExpenseType.Id));
            var expense4 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId4, "name", 18, expenseTypeId: TestExpenseType.Id));
            var sut = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.GetMany(TestUser.Id, 2, 2);

            result.IsError.Should().BeFalse();
            result.Result.TotalItems.Should().Be(4);
            result.Result.Items.Count().Should().Be(2);
            result.Result.Items.Should().Contain(x => x.Id == expense1.Result.Id);
            result.Result.Items.Should().Contain(x => x.Id == expense2.Result.Id);
            result.Result.Items.Should().NotContain(x => x.Id == expense3.Result.Id);
            result.Result.Items.Should().NotContain(x => x.Id == expense4.Result.Id);
        }

        [Fact]
        public async Task WhenGetManyExpensesAggregate_GivenFilters_ThenReturnsPartOfData()
        {
            var initDate = DateTimeOffset.Now;
            var aggId1 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: initDate).Id;
            var aggId2 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: initDate.AddDays(1)).Id;
            var aggId3 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: initDate.AddDays(2)).Id;
            var aggId4 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: initDate.AddDays(3)).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            var expense1 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 10, expenseTypeId: TestExpenseType.Id));
            var expense2 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 20, expenseTypeId: TestExpenseType.Id));
            var expense3 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId3, "name", 30, expenseTypeId: TestExpenseType.Id));
            var expense4 = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId4, "name", 40, expenseTypeId: TestExpenseType.Id));
            var sut = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.GetManyExpensesAggregate(TestUser.Id, 1, fromDate: initDate, toDate: initDate.AddDays(2));

            result.IsError.Should().BeFalse();
            result.Result.TotalItems.Should().Be(3);
            result.Result.Items.Count().Should().Be(3);
            result.Result.Items.Should().Contain(agg => agg.Expenses.Any(x => x.Id == expense1.Result.Id) && agg.TotalExpensesPrice == 10);
            result.Result.Items.Should().Contain(agg => agg.Expenses.Any(x => x.Id == expense2.Result.Id) && agg.TotalExpensesPrice == 20);
            result.Result.Items.Should().Contain(agg => agg.Expenses.Any(x => x.Id == expense3.Result.Id) && agg.TotalExpensesPrice == 30);
            result.Result.Items.All(agg => !agg.Expenses.Any(x => x.Id == expense4.Result.Id)).Should().BeTrue();
        }

        [Fact]
        public async Task WhenGetExpensesAggregateThenReturnsProperValues()
        {
            var expense = _fixture.CreateExpense(TestUser.Id, "name", amount: 3);
            var sut = _factory.Services.GetRequiredService<ExpensesQuery>();

            var result = await sut.GetExpensesAggregate(expense.AggregateId);

            result.Should().NotBeNull();
            result.UserId.Should().Be(TestUser.Id);
            result.Name.Should().Be("expense");
            result.Price.Should().Be(20);
            result.TotalExpensesPrice.Should().Be(60);
            result.IsDraft.Should().BeFalse();
            result.Expenses.Should().ContainSingle();
            result.Expenses.Single().Name.Should().Be("name");
            result.Expenses.Single().Price.Should().Be(20);
            result.Expenses.Single().Amount.Should().Be(3);
            result.Bills.Should().BeEmpty();
        }
    }
}
