using System;
using System.Threading.Tasks;
using BillTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests
{
    public class DevelopmentTests : IClassFixture<BillTrackerFixture>
    {
        private readonly BillTrackerWebApplicationFactory _factory;
        private readonly BillTrackerFixture _fixture;

        public DevelopmentTests(BillTrackerFixture fixture)
        {
            _fixture = fixture;
            _factory = fixture.GetWebApplicationFactory();
        }

        [Fact]
        public async Task CreateBlobContainer()
        {
            var storage = _factory.Services.GetRequiredService<IBillFileStorage>();
        }
    }
}
