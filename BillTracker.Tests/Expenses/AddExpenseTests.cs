using System;
using System.Threading.Tasks;
using BillTracker.Expenses;
using BillTracker.Shared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests.Expenses
{
    public class AddExpenseTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        public AddExpenseTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();
        }

        [Fact]
        public async Task UserWhoNotExistCannotAddExpense()
        {
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IHandle<AddExpenseParameters, ResultOrError<ExpenseModel>>>();

            var result = await sut.Handle(new AddExpenseParameters(Guid.NewGuid(), "name", 20));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.UserNotExist);
        }

        [Fact]
        public async Task UserCanAddExpense()
        {
            var user = await _fixture.CreateUser();
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IHandle<AddExpenseParameters, ResultOrError<ExpenseModel>>>();

            var result = await sut.Handle(new AddExpenseParameters(user.Id, "name", 20));

            result.IsError.Should().BeFalse();
        }
    }
}
