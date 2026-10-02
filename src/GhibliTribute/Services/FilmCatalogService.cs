using GhibliTribute.Constants;
using GhibliTribute.Models;

namespace GhibliTribute.Services
{
    /// <summary>
    ///    Um filme pronto para exibir: definição fixa do catálogo + dados que chegam das APIs.
    /// </summary>
    public class FilmView
    {
        public required FilmDefinition Definition { get; init; }
        public string? Poster { get; set; }
        public int? RuntimeMinutes { get; set; }
        // Nota da crítica: Metacritic (0 a 100)
        public int? CriticScore { get; set; }
        // Nota do público: IMDb (0 a 10)
        public double? AudienceScore { get; set; }
    }

    /// <summary>
    ///    Junta o catálogo fixo com a Studio Ghibli API (capa e duração) e a OMDb (notas, e capa/duração dos filmes que não estão na API).
    ///    Prioridade da capa: arquivo em wwwroot/images/films/{slug}.jpg, capa em alta já usada no carrossel, capa da API do Ghibli, capa da OMDb.
    /// </summary>
    public class FilmCatalogService
    {
        private readonly GhibliApiService _ghibliApi;
        private readonly ImdbApiService _omdb;
        private readonly IWebHostEnvironment _env;

        public FilmCatalogService(GhibliApiService ghibliApi, ImdbApiService omdb, IWebHostEnvironment env)
        {
            _ghibliApi = ghibliApi;
            _omdb = omdb;
            _env = env;
        }

        // Lista inicial, sem chamar API nenhuma, pra página aparecer na hora
        public List<FilmView> GetBaseList() =>
            FilmCatalog.All.Select(definition => new FilmView
            {
                Definition = definition,
                Poster = LocalOverride(definition.Slug) ?? definition.LocalPoster
            }).ToList();

        // Completa a lista com as APIs, tudo em paralelo
        public async Task EnrichAsync(List<FilmView> films)
        {
            var apiFilmsTask = _ghibliApi.GetFilmsAsync();
            var ratingsTask = Task.WhenAll(films.Select(film => _omdb.GetRatingsAsync(film.Definition.ImdbId)));
            await Task.WhenAll(apiFilmsTask, ratingsTask);

            var apiFilms = apiFilmsTask.Result.ToDictionary(f => f.title ?? "", StringComparer.OrdinalIgnoreCase);
            var ratings = ratingsTask.Result;

            for (int i = 0; i < films.Count; i++)
            {
                var film = films[i];
                apiFilms.TryGetValue(film.Definition.GhibliApiTitle ?? "", out var apiFilm);

                film.CriticScore = ratings[i].Metacritic;
                film.AudienceScore = ratings[i].Imdb;
                film.Poster ??= film.Definition.PreferOmdbPoster
                    ? ratings[i].Poster ?? apiFilm?.image
                    : apiFilm?.image ?? ratings[i].Poster;
                film.RuntimeMinutes = int.TryParse(apiFilm?.running_time, out var runtime) ? runtime : ratings[i].RuntimeMinutes;
            }
        }

        private string? LocalOverride(string slug)
        {
            var path = $"images/films/{slug.ToLowerInvariant()}.jpg";
            return _env.WebRootFileProvider.GetFileInfo(path).Exists ? path : null;
        }
    }
}
