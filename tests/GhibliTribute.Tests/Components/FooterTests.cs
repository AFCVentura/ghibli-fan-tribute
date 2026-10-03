using Bunit;
using GhibliTribute.Components;
using GhibliTribute.Tests.Helpers;

namespace GhibliTribute.Tests.Components
{
    /// <summary>
    ///    Renderiza o rodapé com os resx de verdade, pra garantir que os textos existem nas duas línguas.
    /// </summary>
    public class FooterTests : LocalizedBunitContext
    {
        [Theory]
        [InlineData("en-US", "Contact", "Privacy Policy", "belong to Studio Ghibli")]
        [InlineData("pt-BR", "Contato", "Política de Privacidade", "pertencem ao Studio Ghibli")]
        public void RendersTextsInTheCurrentLanguage(string culture, string contact, string privacy, string copyright)
        {
            UseCulture(culture);
            var footer = Render<Footer>();

            Assert.Contains(footer.FindAll("a"), a => a.TextContent.Trim() == contact && a.GetAttribute("href") == "/contact");
            Assert.Contains(footer.FindAll("a"), a => a.TextContent.Trim() == privacy && a.GetAttribute("href") == "/privacy");
            Assert.Contains(copyright, footer.Find("h5").TextContent);
        }

        [Fact]
        public void CopyrightShowsCurrentYear()
        {
            var footer = Render<Footer>();

            Assert.StartsWith($"© {DateTime.Now.Year} ", footer.Find("h5").TextContent);
        }

        [Fact]
        public void ExternalLinksOpenInNewTabWithNoopener()
        {
            var footer = Render<Footer>();

            var external = footer.FindAll("a").Where(a => a.GetAttribute("href")!.StartsWith("http")).ToList();

            Assert.NotEmpty(external);
            Assert.All(external, a =>
            {
                Assert.Equal("_blank", a.GetAttribute("target"));
                Assert.Contains("noopener", a.GetAttribute("rel"));
            });
        }
    }
}
