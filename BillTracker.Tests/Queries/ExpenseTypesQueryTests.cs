using System.Threading.Tasks;
using BillTracker.Commands;
using BillTracker.Entities;
using BillTracker.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests.Queries
{
    public class ExpenseTypesQueryTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;

        public ExpenseTypesQueryTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
        }

        [Fact]
        public async Task WhenGetAllVisibleForUser_ThenReturnsOnlyForUser()
        {
            var createExpenseType = _factory.Services.GetRequiredService<CreateExpenseType>();
            var customType1 = await createExpenseType.Handle(new CreateExpenseTypeParameters(TestUser.Id, "Type 1"));
            var customType2 = await createExpenseType.Handle(new CreateExpenseTypeParameters(TestUser.Id, "Type 2"));
            var sut = _factory.Services.GetRequiredService<ExpenseTypesQuery>();

            var result = await sut.GetAllVisibleForUser(TestUser.Id);

            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Id == customType1.Result.Id);
            result.Should().Contain(x => x.Id == customType2.Result.Id);
        }
    }
}
