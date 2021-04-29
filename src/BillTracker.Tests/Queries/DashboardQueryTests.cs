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
    public class DashboardQueryTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;
        private readonly ExpenseType TestExpenseType1;
        private readonly ExpenseType TestExpenseType2;

        public DashboardQueryTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
            TestExpenseType1 = _fixture.CreateExpenseType(TestUser.Id, "1");
            TestExpenseType2 = _fixture.CreateExpenseType(TestUser.Id, "2");
        }

        [Fact]
        public async Task WhenGet_GivenNoFilters_ThenReturnsMetricsFromAllPeriod()
        {
            var aggId1 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now).Id;
            var aggId2 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(1)).Id;
            var aggId3 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(2)).Id;
            var aggId4 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(3)).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 10, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 20, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId3, "name", 30, expenseTypeId: TestExpenseType1.Id));
            var mostExpensive = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId4, "name", 40, expenseTypeId: TestExpenseType1.Id));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.MostExpensive.Id.Should().Be(mostExpensive.Result.Id);
            dashboard.Result.Metrics.Tranfers.Should().Be(4);
            dashboard.Result.Metrics.Total.Should().Be(100);
        }

        [Fact]
        public async Task WhenGet_GivenNoFilters_ThenReturnsExpenseTypesFromAllPeriod()
        {
            var aggId1 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now).Id;
            var aggId2 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(1)).Id;
            var aggId3 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(2)).Id;
            var aggId4 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(3)).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 10, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 50));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 20, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId3, "name", 30, expenseTypeId: TestExpenseType2.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId4, "name", 40, expenseTypeId: TestExpenseType2.Id));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(2));

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.ExpenseTypes.Should().HaveCount(3);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == TestExpenseType1.Id && x.Total == 30);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == TestExpenseType2.Id && x.Total == 30);
            dashboard.Result.ExpenseTypes.Should().Contain(x => !x.ExpenseTypeId.HasValue && x.Total == 50);
        }

        [Fact]
        public async Task WhenGet_GivenDateFilters_ThenReturnsFilteredMetrics()
        {
            var aggId1 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now).Id;
            var aggId2 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(1)).Id;
            var aggId3 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(2)).Id;
            var aggId4 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(3)).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 10, expenseTypeId: TestExpenseType1.Id));
            var mostExpensiveWithinFilter = await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 20, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId3, "name", 30, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId4, "name", 40, expenseTypeId: TestExpenseType1.Id));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.MostExpensive.Id.Should().Be(mostExpensiveWithinFilter.Result.Id);
            dashboard.Result.Metrics.Tranfers.Should().Be(2);
            dashboard.Result.Metrics.Total.Should().Be(30);
        }

        [Fact]
        public async Task WhenGet_ThenReturnsCalendarDays()
        {
            var aggId1 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now).Id;
            var aggId2 = _fixture.CreateExpenseAggregate(TestUser.Id, addedDate: DateTimeOffset.Now.AddDays(2)).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 10, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId1, "name", 20, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 30, expenseTypeId: TestExpenseType2.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId2, "name", 40, expenseTypeId: TestExpenseType2.Id));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Calendar.Should().HaveCount(2);
            dashboard.Result.Calendar.Should().Contain(x => x.Total == 30);
            dashboard.Result.Calendar.Should().Contain(x => x.Total == 70);
        }

        [Fact]
        public async Task WhenGet_GivenDateFilters_ThenReturnsFilteredExpenseTypes()
        {
            var aggId = _fixture.CreateExpenseAggregate(TestUser.Id).Id;
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", 10, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", 20, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", 30, expenseTypeId: TestExpenseType2.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", 40, expenseTypeId: TestExpenseType2.Id));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.ExpenseTypes.Should().HaveCount(2);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == TestExpenseType1.Id && x.Total == 30);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == TestExpenseType2.Id && x.Total == 70);
        }

        [Fact]
        public async Task WhenGet_GivenUserWithoutExpenses_ThenReturnsEmpty()
        {
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.MostExpensive.Should().BeNull();
            dashboard.Result.Metrics.Tranfers.Should().Be(0);
            dashboard.Result.Metrics.Total.Should().Be(0);
            dashboard.Result.Calendar.Should().BeEmpty();
            dashboard.Result.ExpenseTypes.Should().BeEmpty();
        }

        [Fact]
        public async Task WhenGet_GivenNoFilters_ThenReturnsMetricsFromNoDraftAggregates()
        {
            var addExpenseService = _factory.Services.GetRequiredService<SaveExpense>();
            var aggId = _fixture.CreateExpenseAggregate(TestUser.Id).Id;
            var draftAggId = _fixture.CreateExpenseAggregate(TestUser.Id, isDraft: true).Id;
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", 10, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", 10, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, aggId, "name", 10, 2, expenseTypeId: TestExpenseType1.Id));
            await addExpenseService.Handle(new SaveExpenseParameters(TestUser.Id, draftAggId, "name", 100, expenseTypeId: TestExpenseType1.Id));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.Tranfers.Should().Be(3);
            dashboard.Result.Metrics.Total.Should().Be(40);
            dashboard.Result.ExpenseTypes.First().Total.Should().Be(40);
        }
    }
}
