using System.Globalization;
using GhibliTribute.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace GhibliTribute.Services
{
    /// <summary>
    ///    Nota do IMDb e do Metacritic de um filme, mais a capa e a duração que a OMDb também devolve.
    ///    Null quando o dado não pôde ser obtido (sem chave, API fora do ar, filme sem nota).
    /// </summary>
    public record MovieRatings(double? Imdb, int? Metacritic, string? Poster = null, int? RuntimeMinutes = null);

    /// <summary>
    ///    Service para buscar as notas dos filmes.
    ///    Antes usava a api.imdbapi.dev, mas esse domínio saiu do ar (não resolve mais no DNS), por isso a
    ///    página ficava sem notas. Agora usa a OMDb, que traz a nota do IMDb e o Metascore numa chamada só.
    ///    A chave fica fora do código, em "Omdb:ApiKey" (user-secrets no desenvolvimento, variável de ambiente
    ///    Omdb__ApiKey em produção).
    /// </summary>
    public class ImdbApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ImdbApiService> _logger;
        private readonly string? _apiKey;

        // As notas desses filmes quase não mudam, então 12 horas de cache poupa a cota diária da OMDb
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

        public ImdbApiService(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration, ILogger<ImdbApiService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _apiKey = configuration["Omdb:ApiKey"];
        }

        public async Task<MovieRatings> GetRatingsAsync(string imdbId)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("Omdb:ApiKey não configurada, notas dos filmes não serão exibidas.");
                return new MovieRatings(null, null);
            }

            if (_cache.TryGetValue(imdbId, out MovieRatings? cached) && cached is not null)
                return cached;

            try
            {
                var content = await _httpClient.GetFromJsonAsync<OmdbResponseDTO>($"?i={imdbId}&apikey={_apiKey}");

                if (content is null || content.Response != "True")
                {
                    _logger.LogWarning("OMDb não retornou o filme {ImdbId}: {Error}", imdbId, content?.Error);
                    return new MovieRatings(null, null);
                }

                var ratings = new MovieRatings(
                    ParseDouble(content.imdbRating),
                    ParseInt(content.Metascore),
                    content.Poster is null or "N/A" ? null : content.Poster,
                    ParseInt(content.Runtime?.Replace(" min", "")));
                _cache.Set(imdbId, ratings, CacheDuration);
                return ratings;
            }
            catch (Exception ex)
            {
                // Não guarda em cache, pra tentar de novo na próxima visita
                _logger.LogError(ex, "Erro ao buscar notas do filme {ImdbId}", imdbId);
                return new MovieRatings(null, null);
            }
        }

        private static double? ParseDouble(string? value) =>
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : null;

        private static int? ParseInt(string? value) =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : null;
    }
}
