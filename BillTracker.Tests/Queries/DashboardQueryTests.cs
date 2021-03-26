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
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 10, TestExpenseType1.Id, addedDate: DateTimeOffset.Now));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType1.Id, addedDate: DateTimeOffset.Now.AddDays(1)));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 30, TestExpenseType1.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            var mostExpensive = await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 40, TestExpenseType1.Id, addedDate: DateTimeOffset.Now.AddDays(3)));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.MostExpensive.Id.Should().Be(mostExpensive.Result);
            dashboard.Result.Metrics.Tranfers.Should().Be(4);
            dashboard.Result.Metrics.Total.Should().Be(100);
        }

        [Fact]
        public async Task WhenGet_GivenNoFilters_ThenReturnsExpenseTypesFromAllPeriod()
        {
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 10, TestExpenseType1.Id, addedDate: DateTimeOffset.Now));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType1.Id, addedDate: DateTimeOffset.Now.AddDays(1)));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 30, TestExpenseType2.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 40, TestExpenseType2.Id, addedDate: DateTimeOffset.Now.AddDays(3)));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(2));

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.ExpenseTypes.Should().HaveCount(2);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == TestExpenseType1.Id && x.Total == 30);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == TestExpenseType2.Id && x.Total == 30);
        }

        [Fact]
        public async Task WhenGet_GivenDateFilters_ThenReturnsFilteredMetrics()
        {
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 10, TestExpenseType1.Id, addedDate: DateTimeOffset.Now));
            var mostExpensiveWithinFilter = await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType1.Id, addedDate: DateTimeOffset.Now.AddDays(1)));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 30, TestExpenseType1.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 40, TestExpenseType1.Id, addedDate: DateTimeOffset.Now.AddDays(3)));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.MostExpensive.Id.Should().Be(mostExpensiveWithinFilter.Result);
            dashboard.Result.Metrics.Tranfers.Should().Be(2);
            dashboard.Result.Metrics.Total.Should().Be(30);
        }

        [Fact]
        public async Task WhenGet_ThenReturnsCalendarDays()
        {
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 10, TestExpenseType1.Id, addedDate: DateTimeOffset.Now));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType1.Id, addedDate: DateTimeOffset.Now));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 30, TestExpenseType2.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 40, TestExpenseType2.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
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
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 10, TestExpenseType1.Id));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType1.Id));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 30, TestExpenseType2.Id));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 40, TestExpenseType2.Id));
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
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            var saveAggregate = _factory.Services.GetRequiredService<SaveExpenseAggregate>();
            var draftAggregate = await saveAggregate.Handle(new SaveExpenseAggregateParameters(null, TestUser.Id, "aggregate", isDraft: true));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 10, TestExpenseType1.Id));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 10, TestExpenseType1.Id));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 10, TestExpenseType1.Id));
            await addExpenseService.Handle(new AddExpenseParameters(TestUser.Id, "name", 100, TestExpenseType1.Id, aggregateId: draftAggregate.Result));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(TestUser.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.Tranfers.Should().Be(3);
            dashboard.Result.Metrics.Total.Should().Be(30);
            dashboard.Result.ExpenseTypes.First().Total.Should().Be(30);
        }
    }
}
