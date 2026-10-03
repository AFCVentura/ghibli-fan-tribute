using Bunit;
using GhibliTribute.Components;
using GhibliTribute.Services;
using GhibliTribute.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GhibliTribute.Tests.Components
{
    /// <summary>
    ///    Os botões de tema e idioma da navbar só chamam o JavaScript (theme.js e culture.js).
    ///    O bUnit faz o papel do navegador: cada chamada de JS precisa estar configurada aqui, senão o teste falha.
    /// </summary>
    public class NavMenuTests : LocalizedBunitContext
    {
        private readonly BunitJSModuleInterop _themeModule;
        private readonly BunitJSModuleInterop _cultureModule;

        public NavMenuTests()
        {
            Services.AddScoped<ThemeService>();
            Services.AddScoped<CultureService>();

            _themeModule = JSInterop.SetupModule("./js/theme.js");
            _cultureModule = JSInterop.SetupModule("./js/culture.js");
        }

        [Theory]
        [InlineData("Light", "light")]
        [InlineData("Dark", "dark")]
        [InlineData("System", "system")]
        public void ThemeButton_SavesAndAppliesTheme(string label, string theme)
        {
            _themeModule.SetupVoid("setLocalTheme", theme).SetVoidResult();
            _themeModule.SetupVoid("applyTheme", theme).SetVoidResult();
            var nav = Render<NavMenu>();

            nav.FindAll("button").Single(b => b.TextContent.Trim() == label).Click();

            Assert.Equal(theme, _themeModule.VerifyInvoke("setLocalTheme").Arguments.Single());
            Assert.Equal(theme, _themeModule.VerifyInvoke("applyTheme").Arguments.Single());
        }

        [Theory]
        [InlineData("English", "en-US")]
        [InlineData("Português", "pt-BR")]
        public void LanguageButton_SetsCulture(string label, string culture)
        {
            _cultureModule.SetupVoid("setCulture", culture).SetVoidResult();
            var nav = Render<NavMenu>();

            nav.FindAll("button").Single(b => b.TextContent.Trim() == label).Click();

            Assert.Equal(culture, _cultureModule.VerifyInvoke("setCulture").Arguments.Single());
        }

        [Fact]
        public void HamburgerButton_TogglesMobileMenu()
        {
            var nav = Render<NavMenu>();

            Assert.Contains("max-h-0", nav.Find("ul").ClassName);

            nav.Find("button.md\\:hidden").Click();

            Assert.Contains("max-h-screen", nav.Find("ul").ClassName);
            Assert.DoesNotContain("opacity-0", nav.Find("ul").ClassName);
        }
    }
}
