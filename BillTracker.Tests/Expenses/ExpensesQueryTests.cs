﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillTracker.Expenses;
using BillTracker.Shared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests.Expenses
{
    public class ExpensesQueryTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        public ExpensesQueryTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();
        }

        [Fact]
        public async Task WhenGetByUserId_GivenNonExistingUser_ThenError()
        {
            var sut = _factory.Services.GetRequiredService<IExpensesQuery>();

            var result = await sut.GetByUserId(Guid.NewGuid());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(CommonErrors.UserNotExist);
        }

        [Fact]
        public async Task WhenGetByUserId_GivenFilters_ThenReturnsPartOfData()
        {
            var user = await _fixture.CreateUser();
            var addExpenseService = _factory.Services.GetRequiredService<IAddExpense>();
            var expense1 = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 15, DateTimeOffset.Now));
            var expense2 = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 16, DateTimeOffset.Now.AddDays(1)));
            var expense3 = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 17, DateTimeOffset.Now.AddDays(2)));
            var expense4 = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 18, DateTimeOffset.Now.AddDays(3)));
            var sut = _factory.Services.GetRequiredService<IExpensesQuery>();

            var result = await sut.GetByUserId(user.Id, expense1.Result.AddedAt, expense3.Result.AddedAt);

            result.IsError.Should().BeFalse();
            result.Result.Count().Should().Be(3);
            result.Result.Should().Contain(x => x.Id == expense1.Result.Id);
            result.Result.Should().Contain(x => x.Id == expense2.Result.Id);
            result.Result.Should().Contain(x => x.Id == expense3.Result.Id);
            result.Result.Should().NotContain(x => x.Id == expense4.Result.Id);
        }

        [Fact]
        public async Task WhenGetByUserId_GivenNoFilters_ThenReturnsAllExpenses()
        {
            var user = await _fixture.CreateUser();
            var addExpenseService = _factory.Services.GetRequiredService<IAddExpense>();
            var expense1 = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 15, DateTimeOffset.Now));
            var expense2 = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 16, DateTimeOffset.Now.AddDays(1)));
            var expense3 = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 17, DateTimeOffset.Now.AddDays(2)));
            var expense4 = await addExpenseService.Handle(new AddExpenseParameters(user.Id, "name", 18, DateTimeOffset.Now.AddDays(3)));
            var sut = _factory.Services.GetRequiredService<IExpensesQuery>();

            var result = await sut.GetByUserId(user.Id);

            result.IsError.Should().BeFalse();
            result.Result.Count().Should().Be(4);
            result.Result.Should().Contain(x => x.Id == expense1.Result.Id);
            result.Result.Should().Contain(x => x.Id == expense2.Result.Id);
            result.Result.Should().Contain(x => x.Id == expense3.Result.Id);
            result.Result.Should().Contain(x => x.Id == expense4.Result.Id);
        }
    }
}
