namespace BlazorServerFirstProject.DTOs
{
    /// <summary>
    ///    Recorte da resposta da OMDb (https://www.omdbapi.com) com só o que o site usa.
    ///    A OMDb devolve tudo como texto e usa "N/A" quando não tem o dado.
    /// </summary>
    public class OmdbResponseDTO
    {
        public string? Title { get; set; }
        public string? imdbRating { get; set; }
        public string? Metascore { get; set; }
        public string? Poster { get; set; }
        public string? Runtime { get; set; }
        public string? Response { get; set; }
        public string? Error { get; set; }
    }
}
