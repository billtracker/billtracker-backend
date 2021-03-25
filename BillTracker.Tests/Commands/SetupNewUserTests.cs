using System;
using System.Linq;
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
    public class SetupNewUserTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        public SetupNewUserTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();
        }

        [Fact]
        public async Task CanSetupUser()
        {
            var user = await _fixture.CreateUser();
            var sut = _factory.Services.GetRequiredService<SetupNewUser>();

            var result = await sut.Handle(user.Id);

            result.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task SetupUpUserIsIdempotent()
        {
            var user = await _fixture.CreateUser();
            var sut = _factory.Services.GetRequiredService<SetupNewUser>();

            var result = await sut.Handle(user.Id);
            var result2 = await sut.Handle(user.Id);
            var expenseTypesQuery = _factory.Services.GetRequiredService<ExpenseTypesQuery>();
            var addedExpenses = await expenseTypesQuery.GetAllVisibleForUser(user.Id);

            result.IsError.Should().BeFalse();
            result2.IsError.Should().BeFalse();
            addedExpenses.Count().Should().Be(BuiltInExpenseTypes.Amount);
        }

        [Fact]
        public async Task CannotSetupNonExistingUser()
        {
            var sut = _factory.Services.GetRequiredService<SetupNewUser>();

            var result = await sut.Handle(Guid.NewGuid());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.UserNotExist);
        }
    }
}
