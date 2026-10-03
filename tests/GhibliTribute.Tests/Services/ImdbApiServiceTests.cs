using GhibliTribute.Services;
using GhibliTribute.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace GhibliTribute.Tests.Services
{
    public class ImdbApiServiceTests
    {
        private const string SpiritedAwayJson = """
            {
              "Title": "Spirited Away",
              "imdbRating": "8.6",
              "Metascore": "96",
              "Poster": "https://m.media-amazon.com/spirited.jpg",
              "Runtime": "125 min",
              "Response": "True"
            }
            """;

        private static ImdbApiService CreateService(FakeHttpMessageHandler handler, string? apiKey = "key") =>
            new(handler.CreateClient("https://www.omdbapi.com/"),
                new MemoryCache(new MemoryCacheOptions()),
                TestConfig.Build(("Omdb:ApiKey", apiKey)),
                NullLogger<ImdbApiService>.Instance);

        [Fact]
        public async Task GetRatingsAsync_ParsesRatingsPosterAndRuntime()
        {
            var service = CreateService(FakeHttpMessageHandler.Json(SpiritedAwayJson));

            var ratings = await service.GetRatingsAsync("tt0245429");

            Assert.Equal(new MovieRatings(8.6, 96, "https://m.media-amazon.com/spirited.jpg", 125), ratings);
        }

        [Fact]
        public async Task GetRatingsAsync_TreatsNotAvailableAsNull()
        {
            var service = CreateService(FakeHttpMessageHandler.Json("""
                { "imdbRating": "N/A", "Metascore": "N/A", "Poster": "N/A", "Runtime": "N/A", "Response": "True" }
                """));

            var ratings = await service.GetRatingsAsync("tt0000001");

            Assert.Equal(new MovieRatings(null, null, null, null), ratings);
        }

        [Fact]
        public async Task GetRatingsAsync_WhenFilmNotFound_ReturnsEmpty()
        {
            var service = CreateService(FakeHttpMessageHandler.Json("""{ "Response": "False", "Error": "Incorrect IMDb ID." }"""));

            Assert.Equal(new MovieRatings(null, null), await service.GetRatingsAsync("tt0000001"));
        }

        [Fact]
        public async Task GetRatingsAsync_WithoutApiKey_ReturnsEmptyWithoutCallingOmdb()
        {
            var handler = FakeHttpMessageHandler.Json(SpiritedAwayJson);
            var service = CreateService(handler, apiKey: null);

            Assert.Equal(new MovieRatings(null, null), await service.GetRatingsAsync("tt0245429"));
            Assert.Empty(handler.Requests);
        }

        [Fact]
        public async Task GetRatingsAsync_SendsImdbIdAndApiKey()
        {
            var handler = FakeHttpMessageHandler.Json(SpiritedAwayJson);
            var service = CreateService(handler);

            await service.GetRatingsAsync("tt0245429");

            Assert.Equal("https://www.omdbapi.com/?i=tt0245429&apikey=key", Assert.Single(handler.Requests).Uri.ToString());
        }

        [Fact]
        public async Task GetRatingsAsync_SecondCallForSameFilm_ComesFromCache()
        {
            var handler = FakeHttpMessageHandler.Json(SpiritedAwayJson);
            var service = CreateService(handler);

            var first = await service.GetRatingsAsync("tt0245429");
            var second = await service.GetRatingsAsync("tt0245429");

            Assert.Equal(first, second);
            Assert.Single(handler.Requests);
        }

        [Fact]
        public async Task GetRatingsAsync_AfterNetworkError_TriesAgainNextTime()
        {
            var outage = true;
            var handler = new FakeHttpMessageHandler(_ => outage
                ? throw new HttpRequestException("rede fora do ar")
                : FakeHttpMessageHandler.JsonResponse(SpiritedAwayJson));
            var service = CreateService(handler);

            var duringOutage = await service.GetRatingsAsync("tt0245429");
            outage = false;
            var afterOutage = await service.GetRatingsAsync("tt0245429");

            Assert.Equal(new MovieRatings(null, null), duringOutage);
            Assert.Equal(8.6, afterOutage.Imdb);
            Assert.Equal(2, handler.Requests.Count);
        }
    }
}
