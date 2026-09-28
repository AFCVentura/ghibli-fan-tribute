namespace BlazorServerFirstProject.Services
{
    /// <summary>
    ///    Cloudflare Web Analytics: contagem de visitas sem cookie (por isso não precisa de banner de consentimento).
    ///    O token não é segredo, ele vai no HTML de qualquer jeito, e fica em "CloudflareAnalytics:Token" no appsettings.json.
    ///    Desligado no Development, pra visitas locais não entrarem na contagem.
    ///    O App.razor usa pra incluir o script, e a política de privacidade pra só citar o analytics quando ele está ativo.
    /// </summary>
    public class AnalyticsService
    {
        public string? Token { get; }

        public bool Enabled => !string.IsNullOrWhiteSpace(Token);

        public AnalyticsService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Token = environment.IsDevelopment() ? null : configuration["CloudflareAnalytics:Token"];
        }
    }
}
