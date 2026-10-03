using System.Net;
using System.Text.Json;
using GhibliTribute.Services;
using GhibliTribute.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GhibliTribute.Tests.Services
{
    public class ContactEmailServiceTests
    {
        private static readonly ContactMessage Message = new("Chihiro", "chihiro@example.com", "Freelance", "Hello <b>there</b>");

        private static ContactEmailService CreateService(FakeHttpMessageHandler handler, string environment = "Production",
            string? apiKey = "re_key", string? from = "site@jventura.dev", string? to = "owner@example.com") =>
            new(handler.CreateClient("https://api.resend.com/"),
                TestConfig.Build(("Resend:ApiKey", apiKey), ("Resend:From", from), ("Contact:ToEmail", to)),
                new FakeWebHostEnvironment(environment),
                NullLogger<ContactEmailService>.Instance);

        [Fact]
        public async Task SendAsync_PostsToResendWithBearerKey()
        {
            var handler = FakeHttpMessageHandler.Json("""{ "id": "abc" }""");
            var service = CreateService(handler);

            Assert.True(await service.SendAsync(Message));

            var request = Assert.Single(handler.Requests);
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.resend.com/emails", request.Uri.ToString());
            Assert.Equal("Bearer re_key", request.Authorization);
        }

        [Fact]
        public async Task SendAsync_SendsOnlyToOwner_AndVisitorGoesInReplyTo()
        {
            var handler = FakeHttpMessageHandler.Json("""{ "id": "abc" }""");
            var service = CreateService(handler);

            await service.SendAsync(Message);

            using var json = JsonDocument.Parse(Assert.Single(handler.Requests).Body!);
            var root = json.RootElement;
            Assert.Equal("site@jventura.dev", root.GetProperty("from").GetString());
            Assert.Equal(["owner@example.com"], root.GetProperty("to").EnumerateArray().Select(e => e.GetString()));
            Assert.Equal("chihiro@example.com", root.GetProperty("reply_to").GetString());
            Assert.Equal("[Ghibli Fan Tribute] Freelance | Chihiro", root.GetProperty("subject").GetString());
        }

        [Fact]
        public async Task SendAsync_SendsPlainTextOnly()
        {
            var handler = FakeHttpMessageHandler.Json("""{ "id": "abc" }""");
            var service = CreateService(handler);

            await service.SendAsync(Message);

            using var json = JsonDocument.Parse(Assert.Single(handler.Requests).Body!);
            Assert.False(json.RootElement.TryGetProperty("html", out _));
            Assert.Contains("Hello <b>there</b>", json.RootElement.GetProperty("text").GetString());
        }

        [Fact]
        public async Task SendAsync_WhenResendRejects_ReturnsFalse()
        {
            var service = CreateService(FakeHttpMessageHandler.Json("""{ "message": "invalid from" }""", HttpStatusCode.UnprocessableEntity));

            Assert.False(await service.SendAsync(Message));
        }

        [Fact]
        public async Task SendAsync_OnNetworkError_ReturnsFalse()
        {
            var service = CreateService(FakeHttpMessageHandler.Throws());

            Assert.False(await service.SendAsync(Message));
        }

        [Fact]
        public async Task SendAsync_WithoutConfigInDevelopment_OnlyLogsAndReturnsTrue()
        {
            var handler = FakeHttpMessageHandler.Json("""{ "id": "abc" }""");
            var service = CreateService(handler, "Development", apiKey: null);

            Assert.True(await service.SendAsync(Message));
            Assert.Empty(handler.Requests);
        }

        [Theory]
        [InlineData(null, "site@jventura.dev", "owner@example.com")]
        [InlineData("re_key", null, "owner@example.com")]
        [InlineData("re_key", "site@jventura.dev", null)]
        public async Task SendAsync_WithoutConfigInProduction_ReturnsFalse(string? apiKey, string? from, string? to)
        {
            var handler = FakeHttpMessageHandler.Json("""{ "id": "abc" }""");
            var service = CreateService(handler, "Production", apiKey, from, to);

            Assert.False(await service.SendAsync(Message));
            Assert.Empty(handler.Requests);
        }
    }
}
