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
        private readonly Guid testId = Guid.NewGuid();
        private readonly BillTrackerWebApplicationFactory _factory;

        public IdentityServiceTests(BillTrackerWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CanRegisterIfEmailAddressIsNotTaken()
        {
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            var result = await sut.Register(testId + "test@xyz.com", "pass1", testId.ToString());

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task CantRegisterIfEmailAddressIsTaken()
        {
            var email = testId + "test@xyz.com";
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            await sut.Register(email, "pass1", $"{testId}-1");
            var result = await sut.Register(email, "pass1", $"{testId}-2");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.EmailTaken);
        }

        [Fact]
        public async Task CantRegisterIfUserNameIsTaken()
        {
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            await sut.Register($"{testId}-test1@xyz.com", "pass1", testId.ToString());
            var result = await sut.Register($"{testId}-test2@xyz.com", "pass1", testId.ToString());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.EmailTaken);
        }

        [Fact]
        public async Task CantLoginIfUserIsNotRegistered()
        {
            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            var result = await sut.Login(testId + "test@xyz.com", "pass1");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.InvalidUser);
        }

        [Fact]
        public async Task CanLoginWithUserName()
        {
            var email = testId + "test@xyz.com";
            var userName = testId + "username";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, userName);

            var result = await sut.Login(email, password);

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
            result.Result.AccessToken.Should().NotBeNull();
            result.Result.RefreshToken.Should().NotBeNull();
        }

        [Fact]
        public async Task CanLoginWithEmailAddress()
        {
            var email = testId + "test@xyz.com";
            var userName = testId + "username";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, userName);

            var result = await sut.Login(userName, password);

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
            result.Result.AccessToken.Should().NotBeNull();
            result.Result.RefreshToken.Should().NotBeNull();
        }

        [Fact]
        public async Task CantReuseRefreshTokenWhenWasUsedToRefresh()
        {
            var email = testId + "test@xyz.com";
            var userName = testId + "username";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, userName);

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
            var email = testId + "test@xyz.com";
            var userName = testId + "username";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, userName);
            var loggedIn = await sut.Login(email, password);
            loggedIn.IsError.Should().BeFalse();

            var result = await sut.RevokeToken(loggedIn.Result.RefreshToken);

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task CantRevokeTokenIfNotExist()
        {
            var email = testId + "test@xyz.com";
            var userName = testId + "username";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, userName);
            var loggedIn = await sut.Login(email, password);
            loggedIn.IsError.Should().BeFalse();
            var refreshResult = await sut.RefreshToken(loggedIn.Result.RefreshToken);
            refreshResult.IsError.Should().BeFalse();

            var result = await sut.RevokeToken(loggedIn.Result.RefreshToken);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.InvalidRefreshToken);
        }

        [Fact]
        public async Task CantUseRefreshTokenThatWasCreatedBeforeLogin()
        {
            var email = testId + "test@xyz.com";
            var userName = testId + "username";
            const string password = "pass1";

            using var scope = _factory.Services.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await sut.Register(email, password, userName);

            var loggedIn = await sut.Login(email, password);
            loggedIn.IsError.Should().BeFalse();
            var loggedIn2 = await sut.Login(email, password);
            loggedIn2.IsError.Should().BeFalse();
            var refresh = await sut.RefreshToken(loggedIn.Result.RefreshToken);

            refresh.IsError.Should().BeTrue();
            refresh.Error.Should().Be(IdentityErrors.InvalidRefreshToken);
        }
    }
}
