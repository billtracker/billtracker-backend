using System;
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

        public DashboardQueryTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();
        }

        [Fact]
        public async Task WhenGet_GivenNoFilters_ThenReturnsMetricsFromAllPeriod()
        {
            var user = await _fixture.CreateUser();
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 10, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 20, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(1)));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 30, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            var mostExpensive = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 40, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(3)));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(user.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.MostExpensive.Id.Should().Be(mostExpensive.Result);
            dashboard.Result.Metrics.Tranfers.Should().Be(4);
            dashboard.Result.Metrics.Total.Should().Be(100);
        }

        [Fact]
        public async Task WhenGet_GivenNoFilters_ThenReturnsExpenseTypesFromAllPeriod()
        {
            var user = await _fixture.CreateUser();
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 10, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 20, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(1)));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 30, BuiltInExpenseTypes.Gas.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 40, BuiltInExpenseTypes.Gas.Id, addedDate: DateTimeOffset.Now.AddDays(3)));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(user.Id, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(2));

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.ExpenseTypes.Should().HaveCount(2);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == BuiltInExpenseTypes.Food.Id && x.Total == 30);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == BuiltInExpenseTypes.Gas.Id && x.Total == 30);
        }

        [Fact]
        public async Task WhenGet_GivenDateFilters_ThenReturnsFilteredMetrics()
        {
            var user = await _fixture.CreateUser();
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 10, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now));
            var mostExpensiveWithinFilter = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 20, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(1)));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 30, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 40, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(3)));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(user.Id, DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.MostExpensive.Id.Should().Be(mostExpensiveWithinFilter.Result);
            dashboard.Result.Metrics.Tranfers.Should().Be(2);
            dashboard.Result.Metrics.Total.Should().Be(30);
        }

        [Fact]
        public async Task WhenGet_ThenReturnsCalendarDays()
        {
            var user = await _fixture.CreateUser();
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 10, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 20, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 30, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 40, BuiltInExpenseTypes.Food.Id, addedDate: DateTimeOffset.Now.AddDays(2)));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(user.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Calendar.Should().HaveCount(2);
            dashboard.Result.Calendar.Should().Contain(x => x.Total == 30);
            dashboard.Result.Calendar.Should().Contain(x => x.Total == 70);
        }

        [Fact]
        public async Task WhenGet_GivenDateFilters_ThenReturnsFilteredExpenseTypes()
        {
            var user = await _fixture.CreateUser();
            var addExpenseService = _factory.Services.GetRequiredService<AddExpense>();
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 10, BuiltInExpenseTypes.Food.Id));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 20, BuiltInExpenseTypes.Food.Id));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 30, BuiltInExpenseTypes.Gas.Id));
            await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 40, BuiltInExpenseTypes.Gas.Id));
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(user.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.ExpenseTypes.Should().HaveCount(2);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == BuiltInExpenseTypes.Food.Id && x.Total == 30);
            dashboard.Result.ExpenseTypes.Should().Contain(x => x.ExpenseTypeId == BuiltInExpenseTypes.Gas.Id && x.Total == 70);
        }

        [Fact]
        public async Task WhenGet_GivenUserWithoutExpenses_ThenReturnsEmpty()
        {
            var user = await _fixture.CreateUser();
            var sut = _factory.Services.GetRequiredService<DashboardQuery>();

            var dashboard = await sut.GetDashboard(user.Id);

            dashboard.IsError.Should().BeFalse();
            dashboard.Result.Metrics.MostExpensive.Should().BeNull();
            dashboard.Result.Metrics.Tranfers.Should().Be(0);
            dashboard.Result.Metrics.Total.Should().Be(0);
            dashboard.Result.Calendar.Should().BeEmpty();
            dashboard.Result.ExpenseTypes.Should().BeEmpty();
        }
    }
}
