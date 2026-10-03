using GhibliTribute.Services;
using GhibliTribute.Tests.Helpers;

namespace GhibliTribute.Tests.Services
{
    public class AnalyticsServiceTests
    {
        [Fact]
        public void InProductionWithToken_IsEnabled()
        {
            var service = new AnalyticsService(TestConfig.Build(("CloudflareAnalytics:Token", "abc")), new FakeWebHostEnvironment("Production"));

            Assert.True(service.Enabled);
            Assert.Equal("abc", service.Token);
        }

        [Fact]
        public void InDevelopment_IsDisabledEvenWithToken()
        {
            var service = new AnalyticsService(TestConfig.Build(("CloudflareAnalytics:Token", "abc")), new FakeWebHostEnvironment("Development"));

            Assert.False(service.Enabled);
            Assert.Null(service.Token);
        }

        [Fact]
        public void WithoutToken_IsDisabled()
        {
            var service = new AnalyticsService(TestConfig.Build(), new FakeWebHostEnvironment("Production"));

            Assert.False(service.Enabled);
        }
    }
}
