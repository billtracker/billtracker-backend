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
    public class SaveExpenseAggregateTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;
        private readonly ExpenseType TestExpenseType;

        public SaveExpenseAggregateTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
            TestExpenseType = _fixture.CreateExpenseType(TestUser.Id);
        }

        [Fact]
        public async Task WhenAggregateIdIsNullThenCreatesNew()
        {
            var sut = _factory.Services.GetRequiredService<SaveExpenseAggregate>();

            var result = await sut.Handle(new SaveExpenseAggregateParameters(null, TestUser.Id, "name"));

            result.IsError.Should().BeFalse();
            result.Result.IsDraft.Should().BeFalse();
            result.Result.Expenses.Should().BeEmpty();
            result.Result.Name.Should().Be("name");
            result.Result.TotalAmount.Should().Be(0);
            result.Result.UserId.Should().Be(TestUser.Id);
        }

        [Fact]
        public async Task WhenAggregateIdIsNotNullAndDoesNotExistThenReturnsError()
        {
            var sut = _factory.Services.GetRequiredService<SaveExpenseAggregate>();

            var result = await sut.Handle(new SaveExpenseAggregateParameters(Guid.NewGuid(), TestUser.Id, "name"));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.ExpenseAggregateDoesNotExist);
        }
    }
}
