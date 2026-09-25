using BlazorServerFirstProject.Models;

namespace BlazorServerFirstProject.Constants
{
    /// <summary>
    ///    Lista de filmes exibidos na página /films, em ordem de lançamento.
    /// </summary>
    public static class FilmCatalog
    {
        public static readonly IReadOnlyList<FilmDefinition> All = new List<FilmDefinition>
        {
            // Studio Ghibli
            new("Nausicaa", GhibliMovies.Nausicaa, null, new DateOnly(1984, 3, 11), "Hayao Miyazaki", FilmGroup.Ghibli, NotStudioProduction: true),
            new("Laputa", GhibliMovies.Laputa, "Castle in the Sky", new DateOnly(1986, 8, 2), "Hayao Miyazaki", FilmGroup.Ghibli),
            new("GraveOfTheFireflies", GhibliMovies.GraveOfTheFireflies, "Grave of the Fireflies", new DateOnly(1988, 4, 16), "Isao Takahata", FilmGroup.Ghibli, "images/grave_of_the_fireflies.jpg"),
            new("MyNeighborTotoro", GhibliMovies.MyNeighborTotoro, "My Neighbor Totoro", new DateOnly(1988, 4, 16), "Hayao Miyazaki", FilmGroup.Ghibli, "images/my_neighbor_totoro.jpg"),
            new("KikiDeliveryService", GhibliMovies.KikiDeliveryService, "Kiki's Delivery Service", new DateOnly(1989, 7, 29), "Hayao Miyazaki", FilmGroup.Ghibli),
            new("OnlyYesterday", GhibliMovies.OnlyYesterday, "Only Yesterday", new DateOnly(1991, 7, 20), "Isao Takahata", FilmGroup.Ghibli, PreferOmdbPoster: true), // a capa da API é a do making-of
            new("PorcoRosso", GhibliMovies.PorcoRosso, "Porco Rosso", new DateOnly(1992, 7, 18), "Hayao Miyazaki", FilmGroup.Ghibli),
            new("OceanWaves", GhibliMovies.OceanWaves, null, new DateOnly(1993, 5, 5), "Tomomi Mochizuki", FilmGroup.Ghibli),
            new("PomPoko", GhibliMovies.PomPoko, "Pom Poko", new DateOnly(1994, 7, 16), "Isao Takahata", FilmGroup.Ghibli),
            new("WhisperOfTheHeart", GhibliMovies.WhisperOfTheHeart, "Whisper of the Heart", new DateOnly(1995, 7, 15), "Yoshifumi Kondō", FilmGroup.Ghibli),
            new("PrincessMononoke", GhibliMovies.PrincessMononoke, "Princess Mononoke", new DateOnly(1997, 7, 12), "Hayao Miyazaki", FilmGroup.Ghibli, "images/princess_mononoke.webp"),
            new("MyNeighborsTheYamadas", GhibliMovies.MyNeighborsTheYamadas, "My Neighbors the Yamadas", new DateOnly(1999, 7, 17), "Isao Takahata", FilmGroup.Ghibli),
            new("SpiritedAway", GhibliMovies.SpiritedAway, "Spirited Away", new DateOnly(2001, 7, 20), "Hayao Miyazaki", FilmGroup.Ghibli, "images/spirited_away.jpg"),
            new("TheCatReturns", GhibliMovies.TheCatReturns, "The Cat Returns", new DateOnly(2002, 7, 19), "Hiroyuki Morita", FilmGroup.Ghibli),
            new("HowlsMovingCastle", GhibliMovies.HowlsMovingCastle, "Howl's Moving Castle", new DateOnly(2004, 11, 20), "Hayao Miyazaki", FilmGroup.Ghibli),
            new("TalesFromEarthsea", GhibliMovies.TalesFromEarthsea, "Tales from Earthsea", new DateOnly(2006, 7, 29), "Gorō Miyazaki", FilmGroup.Ghibli),
            new("Ponyo", GhibliMovies.Ponyo, "Ponyo", new DateOnly(2008, 7, 19), "Hayao Miyazaki", FilmGroup.Ghibli),
            new("Arrietty", GhibliMovies.Arrietty, "Arrietty", new DateOnly(2010, 7, 17), "Hiromasa Yonebayashi", FilmGroup.Ghibli),
            new("FromUpOnPoppyHill", GhibliMovies.FromUpOnPoppyHill, "From Up on Poppy Hill", new DateOnly(2011, 7, 16), "Gorō Miyazaki", FilmGroup.Ghibli),
            new("TheWindRises", GhibliMovies.TheWindRises, "The Wind Rises", new DateOnly(2013, 7, 20), "Hayao Miyazaki", FilmGroup.Ghibli),
            new("TheTaleOfThePrincessKaguya", GhibliMovies.TheTaleOfThePrincessKaguya, "The Tale of the Princess Kaguya", new DateOnly(2013, 11, 23), "Isao Takahata", FilmGroup.Ghibli),
            new("WhenMarnieWasThere", GhibliMovies.WhenMarnieWasThere, "When Marnie Was There", new DateOnly(2014, 7, 19), "Hiromasa Yonebayashi", FilmGroup.Ghibli),
            new("TheRedTurtle", GhibliMovies.TheRedTurtle, "The Red Turtle", new DateOnly(2016, 9, 17), "Michaël Dudok de Wit", FilmGroup.Ghibli),
            new("EarwigAndTheWitch", GhibliMovies.EarwigAndTheWitch, "Earwig and the Witch", new DateOnly(2020, 12, 30), "Gorō Miyazaki", FilmGroup.Ghibli),
            new("TheBoyAndTheHeron", GhibliMovies.TheBoyAndTheHeron, null, new DateOnly(2023, 7, 14), "Hayao Miyazaki", FilmGroup.Ghibli, "images/the_boy_and_the_heron.jpg"),

            // Antes e além do Ghibli
            new("Horus", GhibliMovies.Horus, null, new DateOnly(1968, 7, 21), "Isao Takahata", FilmGroup.Related),
            new("PussInBoots", GhibliMovies.PussInBoots, null, new DateOnly(1969, 3, 18), "Kimio Yabuki", FilmGroup.Related),
            new("PandaGoPanda", GhibliMovies.PandaGoPanda, null, new DateOnly(1972, 12, 17), "Isao Takahata", FilmGroup.Related),
            new("Cagliostro", GhibliMovies.Cagliostro, null, new DateOnly(1979, 12, 15), "Hayao Miyazaki", FilmGroup.Related),
            new("ChieTheBrat", GhibliMovies.ChieTheBrat, null, new DateOnly(1981, 4, 11), "Isao Takahata", FilmGroup.Related),
            new("GaucheTheCellist", GhibliMovies.GaucheTheCellist, null, new DateOnly(1982, 1, 23), "Isao Takahata", FilmGroup.Related),
            new("Yanagawa", GhibliMovies.Yanagawa, null, new DateOnly(1987, 8, 15), "Isao Takahata", FilmGroup.Related),
        };
    }
}
