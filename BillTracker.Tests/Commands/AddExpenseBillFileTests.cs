using System;
using System.IO;
using System.Threading.Tasks;
using BillTracker.Commands;
using BillTracker.Entities;
using BillTracker.Tests.TestClasses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests.Commands
{
    public class AddExpenseBillFileTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        private readonly User TestUser;
        private readonly Expense TestExpense;

        public AddExpenseBillFileTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();

            TestUser = _fixture.CreateUser();
            TestExpense = _fixture.CreateExpense(TestUser.Id);
        }

        [Fact]
        public async Task CanUploadBillFile()
        {
            var sut = _factory.Services.GetRequiredService<AddExpenseBillFile>();

            var result = await sut.Handle(new AddBillFileParameters(TestUser.Id, TestExpense.AggregateId, GetTestFile("bill.jpg"), "bill.jpg"));

            result.IsError.Should().BeFalse();
            result.Result.AggregateId.Should().Be(TestExpense.AggregateId);
            result.Result.FileName.Should().Be("0-bill.jpg");
        }

        [Fact]
        public async Task CannotUploadFileBiggerThan10MB()
        {
            var sut = _factory.Services.GetRequiredService<AddExpenseBillFile>();

            var result = await sut.Handle(new AddBillFileParameters(Guid.NewGuid(), Guid.NewGuid(), new FakeStream(size: 1000000000), null));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(AddExpenseBillFile.BillFileIsTooBig);
        }

        [Fact]
        public async Task FileMustBeProvided()
        {
            var sut = _factory.Services.GetRequiredService<AddExpenseBillFile>();

            var result = await sut.Handle(new AddBillFileParameters(Guid.NewGuid(), Guid.NewGuid(), null, null));

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(AddExpenseBillFile.BillFileIsEmpty);
        }

        private static FileStream GetTestFile(string testFileName) => File.OpenRead(Path.Combine("TestFiles", testFileName));
    }
}
