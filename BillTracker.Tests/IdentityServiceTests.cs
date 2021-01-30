using System;
using System.Threading.Tasks;
using BillTracker.Identity;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests
{
    public class IdentityServiceTests : IClassFixture<BillTrackerWebApplicationFactory>
    {
        private readonly BillTrackerWebApplicationFactory _factory;

        private readonly Guid TestId = Guid.NewGuid();

        public IdentityServiceTests(BillTrackerWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CanRegisterIfEmailAddressIsNotTaken()
        {
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            var result = await sut.Register(TestId + "test@xyz.com", "pass1", "name", "last");

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task CantRegisterIfEmailAddressIsTaken()
        {
            var email = TestId + "test@xyz.com";
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            await sut.Register(email, "pass1", "name", "last");
            var result = await sut.Register(email, "pass1", "name", "last");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.EmailTaken);
        }

        [Fact]
        public async Task CantLoginIfUserIsNotRegistered()
        {
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            var result = await sut.Login(TestId + "test@xyz.com", "pass1");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.InvalidUser);
        }

        [Fact]
        public async Task CanLoginIfParametersAreOk()
        {
            var email = TestId + "test@xyz.com";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, "name", "last");

            var result = await sut.Login(email, password);

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
            result.Result.AccessToken.Should().NotBeNull();
            result.Result.RefreshToken.Should().NotBeNull();
        }

        [Fact]
        public async Task CantReuseRefreshToken()
        {
            var email = TestId + "test@xyz.com";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, "name", "last");

            var loggedIn = await sut.Login(email, password);
            loggedIn.IsError.Should().BeFalse();
            var refresh1 = await sut.RefreshToken(loggedIn.Result.RefreshToken);
            var refresh2 = await sut.RefreshToken(loggedIn.Result.RefreshToken);

            refresh1.IsError.Should().BeFalse();
            refresh1.Error.Should().BeNullOrEmpty();
            refresh1.Result.AccessToken.Should().NotBeNull();
            refresh1.Result.RefreshToken.Should().NotBeNull();
            refresh2.IsError.Should().BeTrue();
            refresh2.Error.Should().Be(IdentityErrors.InvalidRefreshToken);
        }

        [Fact]
        public async Task CanRevokeToken()
        {
            var email = TestId + "test@xyz.com";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, "name", "last");
            var loggedIn = await sut.Login(email, password);
            loggedIn.IsError.Should().BeFalse();

            var result = await sut.RevokeToken(loggedIn.Result.RefreshToken);

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task CantRevokeTokenIfNotExist()
        {
            var email = TestId + "test@xyz.com";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, "name", "last");
            var loggedIn = await sut.Login(email, password);
            loggedIn.IsError.Should().BeFalse();
            var refreshResult = await sut.RefreshToken(loggedIn.Result.RefreshToken);
            refreshResult.IsError.Should().BeFalse();

            var result = await sut.RevokeToken(loggedIn.Result.RefreshToken);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.InvalidRefreshToken);
        }
    }
}
