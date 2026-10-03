using GhibliTribute.Services;
using GhibliTribute.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GhibliTribute.Tests.Services
{
    public class TurnstileServiceTests
    {
        private const string Approved = """{ "success": true, "error-codes": [] }""";

        private static TurnstileService CreateService(FakeHttpMessageHandler handler, string? secretKey = "secret", string? siteKey = "site") =>
            new(handler.CreateClient("https://challenges.cloudflare.com/"),
                TestConfig.Build(("Turnstile:SecretKey", secretKey), ("Turnstile:SiteKey", siteKey)),
                NullLogger<TurnstileService>.Instance);

        [Theory]
        [InlineData(null, "site")]
        [InlineData("secret", null)]
        [InlineData("", "")]
        public async Task VerifyAsync_WithoutKeys_ReturnsFalseWithoutCallingCloudflare(string? secretKey, string? siteKey)
        {
            var handler = FakeHttpMessageHandler.Json(Approved);
            var service = CreateService(handler, secretKey, siteKey);

            var result = await service.VerifyAsync("token", "1.2.3.4");

            Assert.False(result);
            Assert.Empty(handler.Requests);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task VerifyAsync_WithEmptyToken_ReturnsFalseWithoutCallingCloudflare(string? token)
        {
            var handler = FakeHttpMessageHandler.Json(Approved);
            var service = CreateService(handler);

            var result = await service.VerifyAsync(token, "1.2.3.4");

            Assert.False(result);
            Assert.Empty(handler.Requests);
        }

        [Fact]
        public async Task VerifyAsync_WhenCloudflareApproves_ReturnsTrue()
        {
            var service = CreateService(FakeHttpMessageHandler.Json(Approved));

            Assert.True(await service.VerifyAsync("token", "1.2.3.4"));
        }

        [Fact]
        public async Task VerifyAsync_WhenCloudflareRejects_ReturnsFalse()
        {
            var service = CreateService(FakeHttpMessageHandler.Json("""{ "success": false, "error-codes": ["invalid-input-response"] }"""));

            Assert.False(await service.VerifyAsync("token", "1.2.3.4"));
        }

        [Fact]
        public async Task VerifyAsync_OnNetworkError_ReturnsFalse()
        {
            var service = CreateService(FakeHttpMessageHandler.Throws());

            Assert.False(await service.VerifyAsync("token", "1.2.3.4"));
        }

        [Fact]
        public async Task VerifyAsync_PostsSecretTokenAndIpToSiteverify()
        {
            var handler = FakeHttpMessageHandler.Json(Approved);
            var service = CreateService(handler);

            await service.VerifyAsync("the-token", "1.2.3.4");

            var request = Assert.Single(handler.Requests);
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://challenges.cloudflare.com/turnstile/v0/siteverify", request.Uri.ToString());
            Assert.Equal("secret=secret&response=the-token&remoteip=1.2.3.4", request.Body);
        }

        [Fact]
        public async Task VerifyAsync_WithoutIp_LeavesRemoteipOut()
        {
            var handler = FakeHttpMessageHandler.Json(Approved);
            var service = CreateService(handler);

            await service.VerifyAsync("the-token", null);

            Assert.Equal("secret=secret&response=the-token", Assert.Single(handler.Requests).Body);
        }
    }
}
