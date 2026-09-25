namespace BlazorServerFirstProject.Models
{
    public enum FilmGroup
    {
        // Filmes do Studio Ghibli (Nausicaä entra aqui mesmo sendo anterior ao estúdio)
        Ghibli,
        // Filmes dos fundadores feitos antes ou fora do estúdio
        Related
    }

    /// <summary>
    ///    Definição fixa de um filme no catálogo do site.
    ///    Título e sinopse ficam nos resx (Film_{Slug}_Title / Film_{Slug}_Synopsis), porque a API do Ghibli só tem texto em inglês.
    /// </summary>
    /// <param name="Slug">Chave usada nos resx e no nome da capa local opcional (wwwroot/images/films/{slug}.jpg).</param>
    /// <param name="ImdbId">Id do IMDb, usado para buscar as notas na OMDb.</param>
    /// <param name="GhibliApiTitle">Título exato na Studio Ghibli API, quando o filme existe nela.</param>
    /// <param name="Released">Data de estreia no Japão, usada na ordenação por lançamento.</param>
    /// <param name="Director">Diretor(es).</param>
    /// <param name="Group">Se é do Ghibli ou relacionado aos fundadores.</param>
    /// <param name="LocalPoster">Capa em alta que já existe no projeto (as mesmas do carrossel da Home).</param>
    /// <param name="NotStudioProduction">Marca o Nausicaä, que aparece junto do Ghibli com uma observação.</param>
    /// <param name="PreferOmdbPoster">Usa a capa da OMDb mesmo com o filme na API do Ghibli (a API tem capa errada para alguns).</param>
    public record FilmDefinition(
        string Slug,
        string ImdbId,
        string? GhibliApiTitle,
        DateOnly Released,
        string Director,
        FilmGroup Group,
        string? LocalPoster = null,
        bool NotStudioProduction = false,
        bool PreferOmdbPoster = false);
}
