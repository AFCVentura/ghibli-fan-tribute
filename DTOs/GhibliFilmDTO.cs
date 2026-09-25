namespace BlazorServerFirstProject.DTOs
{
    /// <summary>
    ///    Recorte de um filme da Studio Ghibli API (https://ghibliapi.vercel.app/films).
    /// </summary>
    public class GhibliFilmDTO
    {
        public string? id { get; set; }
        public string? title { get; set; }
        public string? image { get; set; }
        public string? movie_banner { get; set; }
        public string? description { get; set; }
        public string? director { get; set; }
        public string? release_date { get; set; }
        public string? running_time { get; set; }
        public string? rt_score { get; set; }
    }
}
