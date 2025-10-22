namespace BlazorServerFirstProject.DTOs
{
    public class Director
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public List<string> alternativeNames { get; set; }
        public PrimaryImage primaryImage { get; set; }
        public List<string> primaryProfessions { get; set; }
    }

    public class Interest
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool? isSubgenre { get; set; }
    }

    public class Metacritic
    {
        public int score { get; set; }
        public int reviewCount { get; set; }
    }

    public class OriginCountry
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class PrimaryImage
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Rating
    {
        public double aggregateRating { get; set; }
        public int voteCount { get; set; }
    }

    public class ImdbApiResponseDTO
    {
        public string id { get; set; }
        public string type { get; set; }
        public string primaryTitle { get; set; }
        public string originalTitle { get; set; }
        public PrimaryImage primaryImage { get; set; }
        public int startYear { get; set; }
        public int runtimeSeconds { get; set; }
        public List<string> genres { get; set; }
        public Rating rating { get; set; }
        public Metacritic metacritic { get; set; }
        public string plot { get; set; }
        public List<Director> directors { get; set; }
        public List<Writer> writers { get; set; }
        public List<Star> stars { get; set; }
        public List<OriginCountry> originCountries { get; set; }
        public List<SpokenLanguage> spokenLanguages { get; set; }
        public List<Interest> interests { get; set; }
    }

    public class SpokenLanguage
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Star
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public List<string> alternativeNames { get; set; }
        public PrimaryImage primaryImage { get; set; }
        public List<string> primaryProfessions { get; set; }
    }

    public class Writer
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public List<string> alternativeNames { get; set; }
        public PrimaryImage primaryImage { get; set; }
        public List<string> primaryProfessions { get; set; }
    }


}
