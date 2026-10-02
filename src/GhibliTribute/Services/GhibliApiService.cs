using GhibliTribute.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace GhibliTribute.Services
{
    /// <summary>
    ///    Service para consumir a Studio Ghibli API, de onde vêm as capas e a duração dos filmes.
    ///    A API é estática (não muda), então a lista fica 24 horas em cache.
    /// </summary>
    public class GhibliApiService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<GhibliApiService> _logger;

        public GhibliApiService(HttpClient httpClient, IMemoryCache cache, ILogger<GhibliApiService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public Task<IReadOnlyList<GhibliFilmDTO>> GetFilmsAsync() => GetListAsync<GhibliFilmDTO>("films");

        // Lista inteira do endpoint, sem paginação, guardada em cache pelo nome do endpoint
        private async Task<IReadOnlyList<T>> GetListAsync<T>(string endpoint)
        {
            var cacheKey = $"ghibli-api-{endpoint}";
            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<T>? cached) && cached is not null)
                return cached;

            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<T>>(endpoint) ?? new();
                _cache.Set(cacheKey, (IReadOnlyList<T>)items, CacheDuration);
                return items;
            }
            catch (Exception ex)
            {
                // Sem a API a página de filmes continua funcionando, só usa as capas da OMDb
                _logger.LogError(ex, "Erro ao buscar {Endpoint} na Studio Ghibli API", endpoint);
                return Array.Empty<T>();
            }
        }
    }
}
