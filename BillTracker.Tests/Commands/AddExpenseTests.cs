using System;
using System.Threading.Tasks;
using BillTracker.Commands;
using BillTracker.Entities;
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
        public async Task UserCanAddExpenseWithCustomType()
        {
            var sut = _factory.Services.GetRequiredService<AddExpense>();

            var result = await sut.Handle(new AddExpenseParameters(TestUser.Id, "name", 20, TestExpenseType.Id));

            result.IsError.Should().BeFalse();
            result.Result.Should().NotBeEmpty();
        }
    }
}
