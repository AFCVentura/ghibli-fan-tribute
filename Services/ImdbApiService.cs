using BlazorServerFirstProject.DTOs;

namespace BlazorServerFirstProject.Services
{
    /// <summary>
    ///    Service para consumir a API do IMDB e retornar notas dos filmes.
    /// </summary>
    public class ImdbApiService
    {
        private HttpClient _httpClient { get; set; } = new HttpClient();
        private const string _baseUrl = "https://api.imdbapi.dev";

        public ImdbApiService()
        {
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<double> GetMovieRatingAsync(string imdbId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/titles/{imdbId}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadFromJsonAsync<ImdbApiResponseDTO>();
                return content?.rating?.aggregateRating ?? 0.0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching movie rating: {ex.Message}");
                return 0.0;
            }
        }
        public async Task<int> GetMovieMetacriticAsync(string imdbId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/titles/{imdbId}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadFromJsonAsync<ImdbApiResponseDTO>();
                return content?.metacritic?.score?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching movie rating: {ex.Message}");
                return 0;
            }
        }


    }
}
