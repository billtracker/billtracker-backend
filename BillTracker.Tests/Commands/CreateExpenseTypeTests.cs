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

        public CreateExpenseTypeTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();
        }

        [Fact]
        public async Task UserCanAddExpenseTypeIfNameDoesNotExist()
        {
            var user = await _fixture.CreateUser();
            var sut = _factory.Services.GetRequiredService<CreateExpenseType>();

            var result = await sut.Handle(new CreateExpenseTypeParameters(user.Id, "Custom name"));

            result.IsError.Should().BeFalse();
            result.Result.Name.Should().Be("Custom name");
            result.Result.IsBuiltIn.Should().BeFalse();
        }

        [Fact]
        public async Task UserCannotAddExpenseTypeIfNameExist()
        {
            var user = await _fixture.CreateUser();
            var sut = _factory.Services.GetRequiredService<CreateExpenseType>();
            await sut.Handle(new CreateExpenseTypeParameters(user.Id, "Custom name"));

            var result1 = await sut.Handle(new CreateExpenseTypeParameters(user.Id, "Custom name"));
            var result2 = await sut.Handle(new CreateExpenseTypeParameters(user.Id, BuiltInExpenseTypes.Food.Name));

            result1.IsError.Should().BeTrue();
            result1.Error.Should().Be(CreateExpenseType.ExpenseTypeAlreadyExist);
            result2.IsError.Should().BeTrue();
            result2.Error.Should().Be(CreateExpenseType.ExpenseTypeAlreadyExist);
        }
    }
}
