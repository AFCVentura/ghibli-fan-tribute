using System.Web;
using GhibliTribute.Services;
using GhibliTribute.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace GhibliTribute.Tests.Services
{
    /// <summary>
    ///    Testa a junção do catálogo fixo com as duas APIs: de onde vem a capa e a duração de cada filme.
    ///    Usa filmes reais do catálogo, cada um representando uma regra (Laputa: filme comum na API do Ghibli;
    ///    OnlyYesterday: PreferOmdbPoster; GraveOfTheFireflies: capa local; Nausicaa: fora da API do Ghibli).
    /// </summary>
    public class FilmCatalogServiceTests : IDisposable
    {
        private const string GhibliApiJson = """
            [
              { "title": "Castle in the Sky", "image": "https://ghibli/laputa.jpg", "running_time": "124" },
              { "title": "Only Yesterday", "image": "https://ghibli/only-yesterday-making-of.jpg", "running_time": "118" },
              { "title": "Grave of the Fireflies", "image": "https://ghibli/grave.jpg", "running_time": "89" }
            ]
            """;

        // Cada filme responde com valores derivados do próprio id, pra dar pra saber de onde veio cada dado
        private readonly FakeHttpMessageHandler _omdb = new(request =>
        {
            var imdbId = HttpUtility.ParseQueryString(request.RequestUri!.Query)["i"];
            return FakeHttpMessageHandler.JsonResponse($$"""
                { "imdbRating": "8.1", "Metascore": "85", "Poster": "https://omdb/{{imdbId}}.jpg", "Runtime": "100 min", "Response": "True" }
                """);
        });

        private readonly string _webRoot = Directory.CreateTempSubdirectory("ghibli-wwwroot-").FullName;

        public void Dispose() => Directory.Delete(_webRoot, recursive: true);

        private FilmCatalogService CreateService(FakeHttpMessageHandler ghibliApi)
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            return new FilmCatalogService(
                new GhibliApiService(ghibliApi.CreateClient("https://ghibliapi.vercel.app/"), cache, NullLogger<GhibliApiService>.Instance),
                new ImdbApiService(_omdb.CreateClient("https://www.omdbapi.com/"), cache, TestConfig.Build(("Omdb:ApiKey", "key")), NullLogger<ImdbApiService>.Instance),
                new FakeWebHostEnvironment(webRootPath: _webRoot));
        }

        private async Task<List<FilmView>> LoadAsync(FakeHttpMessageHandler ghibliApi)
        {
            var service = CreateService(ghibliApi);
            var films = service.GetBaseList();
            await service.EnrichAsync(films);
            return films;
        }

        private static FilmView Film(List<FilmView> films, string slug) => films.Single(f => f.Definition.Slug == slug);

        [Fact]
        public async Task EnrichAsync_FilmInGhibliApi_UsesGhibliPosterAndRuntime()
        {
            var laputa = Film(await LoadAsync(FakeHttpMessageHandler.Json(GhibliApiJson)), "Laputa");

            Assert.Equal("https://ghibli/laputa.jpg", laputa.Poster);
            Assert.Equal(124, laputa.RuntimeMinutes);
        }

        [Fact]
        public async Task EnrichAsync_PreferOmdbPoster_UsesOmdbPosterEvenWithGhibliPoster()
        {
            var onlyYesterday = Film(await LoadAsync(FakeHttpMessageHandler.Json(GhibliApiJson)), "OnlyYesterday");

            Assert.Equal($"https://omdb/{onlyYesterday.Definition.ImdbId}.jpg", onlyYesterday.Poster);
            Assert.Equal(118, onlyYesterday.RuntimeMinutes);
        }

        [Fact]
        public async Task EnrichAsync_FilmOutsideGhibliApi_UsesOmdbPosterAndRuntime()
        {
            var nausicaa = Film(await LoadAsync(FakeHttpMessageHandler.Json(GhibliApiJson)), "Nausicaa");

            Assert.Equal($"https://omdb/{nausicaa.Definition.ImdbId}.jpg", nausicaa.Poster);
            Assert.Equal(100, nausicaa.RuntimeMinutes);
        }

        [Fact]
        public async Task EnrichAsync_FilmWithLocalPoster_KeepsLocalPoster()
        {
            var grave = Film(await LoadAsync(FakeHttpMessageHandler.Json(GhibliApiJson)), "GraveOfTheFireflies");

            Assert.Equal("images/grave_of_the_fireflies.jpg", grave.Poster);
            Assert.Equal(89, grave.RuntimeMinutes);
        }

        [Fact]
        public async Task GetBaseList_PosterFileInWwwroot_WinsOverEverything()
        {
            Directory.CreateDirectory(Path.Combine(_webRoot, "images", "films"));
            File.WriteAllBytes(Path.Combine(_webRoot, "images", "films", "laputa.jpg"), []);

            var laputa = Film(await LoadAsync(FakeHttpMessageHandler.Json(GhibliApiJson)), "Laputa");

            Assert.Equal("images/films/laputa.jpg", laputa.Poster);
        }

        [Fact]
        public async Task EnrichAsync_WhenGhibliApiIsDown_FallsBackToOmdb()
        {
            var laputa = Film(await LoadAsync(FakeHttpMessageHandler.Throws()), "Laputa");

            Assert.Equal($"https://omdb/{laputa.Definition.ImdbId}.jpg", laputa.Poster);
            Assert.Equal(100, laputa.RuntimeMinutes);
        }

        [Fact]
        public async Task EnrichAsync_MapsMetascoreToCriticAndImdbToAudience()
        {
            var laputa = Film(await LoadAsync(FakeHttpMessageHandler.Json(GhibliApiJson)), "Laputa");

            Assert.Equal(85, laputa.CriticScore);
            Assert.Equal(8.1, laputa.AudienceScore);
        }
    }
}
