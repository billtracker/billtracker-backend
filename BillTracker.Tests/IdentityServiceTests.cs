using System.Threading.Tasks;
using BillTracker.Identity;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BillTracker.Tests
{
    public class IdentityServiceTests
    {
        private readonly DbContextMock<BillTrackerContext> _dbContextMock =
            new DbContextMock<BillTrackerContext>(new DbContextOptions<BillTrackerContext>());

        public IdentityServiceTests()
        {
            _dbContextMock.CreateDbSetMock(m => m.Users);
        }

        [Fact]
        public async Task CanRegisterIfEmailAddressIsNotTaken()
        {
            var sut = new IdentityService(_dbContextMock.Object);

            var result = await sut.Register("test@xyz.com", "pass1", "name", "last");

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task CantRegisterIfEmailAddressIsTaken()
        {
            var sut = new IdentityService(_dbContextMock.Object);

            await sut.Register("test@xyz.com", "pass1", "name", "last");
            var result = await sut.Register("test@xyz.com", "pass1", "name", "last");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.EmailTaken);
        }

        [Fact]
        public async Task CantLoginIfUserIsNotRegistered()
        {
            var sut = new IdentityService(_dbContextMock.Object);

            var result = await sut.Login("test@xyz.com", "pass1");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(IdentityErrors.InvalidUser);
        }

        [Fact]
        public async Task CanLoginIfParametersAreOk()
        {
            const string email = "test@xyz.com";
            const string password = "pass1";

            var sut = new IdentityService(_dbContextMock.Object);
            await sut.Register(email, password, "name", "last");

            var result = await sut.Login(email, password);

            result.IsError.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();
            result.Result.Should().NotBeNull();
        }
    }
}
