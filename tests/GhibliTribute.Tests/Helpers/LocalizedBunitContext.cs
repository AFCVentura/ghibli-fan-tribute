using System.Globalization;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace GhibliTribute.Tests.Helpers
{
    /// <summary>
    ///    Contexto do bUnit com os resx de verdade e a cultura escolhida pelo teste (padrão en-US),
    ///    pra o resultado não depender do idioma do Windows de quem roda. A cultura original volta no Dispose.
    /// </summary>
    public abstract class LocalizedBunitContext : BunitContext
    {
        private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;
        private readonly CultureInfo _originalUICulture = CultureInfo.CurrentUICulture;

        protected LocalizedBunitContext()
        {
            Services.AddLocalization(options => options.ResourcesPath = "Resources");
            UseCulture("en-US");
        }

        protected static void UseCulture(string culture) =>
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo(culture);

        protected override void Dispose(bool disposing)
        {
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUICulture;
            base.Dispose(disposing);
        }
    }
}
