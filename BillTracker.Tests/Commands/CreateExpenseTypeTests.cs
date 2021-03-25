using System.Threading.Tasks;
using BillTracker.Commands;
using BillTracker.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests.Commands
{
    public class CreateExpenseTypeTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;

        public CreateExpenseTypeTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
        }

        [Fact]
        public async Task UserCanAddExpenseTypeIfNameDoesNotExist()
        {
            var sut = _factory.Services.GetRequiredService<CreateExpenseType>();

            var result = await sut.Handle(new CreateExpenseTypeParameters(TestUser.Id, "Custom name"));

            result.IsError.Should().BeFalse();
            result.Result.Name.Should().Be("Custom name");
            result.Result.IsBuiltIn.Should().BeFalse();
        }

        [Fact]
        public async Task UserCannotAddExpenseTypeIfNameExist()
        {
            var sut = _factory.Services.GetRequiredService<CreateExpenseType>();
            await sut.Handle(new CreateExpenseTypeParameters(TestUser.Id, "Custom name"));

            var result = await sut.Handle(new CreateExpenseTypeParameters(TestUser.Id, "Custom name"));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CreateExpenseType.ExpenseTypeAlreadyExist);
        }
    }
}
