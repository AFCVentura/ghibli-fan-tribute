using System.Text.Json.Serialization;

namespace GhibliTribute.Services
{
    /// <summary>
    ///    Confere no servidor da Cloudflare o token que o widget do Turnstile (o "CAPTCHA" invisível) colocou
    ///    no formulário de contato. O token sozinho não prova nada: só a Cloudflare sabe se ele é válido.
    ///    Chaves em "Turnstile:SiteKey" (vai no HTML) e "Turnstile:SecretKey" (só no servidor). No desenvolvimento
    ///    o appsettings.Development.json usa as chaves de teste oficiais, que sempre aprovam.
    /// </summary>
    public class TurnstileService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TurnstileService> _logger;
        private readonly string? _secretKey;

        public string? SiteKey { get; }

        public TurnstileService(HttpClient httpClient, IConfiguration configuration, ILogger<TurnstileService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _secretKey = configuration["Turnstile:SecretKey"];
            SiteKey = configuration["Turnstile:SiteKey"];
        }

        public async Task<bool> VerifyAsync(string? token, string? remoteIp)
        {
            if (string.IsNullOrWhiteSpace(_secretKey) || string.IsNullOrWhiteSpace(SiteKey))
            {
                _logger.LogError("Turnstile:SiteKey ou Turnstile:SecretKey não configurada, formulário de contato bloqueado.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                var fields = new Dictionary<string, string> { ["secret"] = _secretKey, ["response"] = token };
                if (!string.IsNullOrEmpty(remoteIp))
                    fields["remoteip"] = remoteIp;

                using var response = await _httpClient.PostAsync("turnstile/v0/siteverify", new FormUrlEncodedContent(fields));
                var result = await response.Content.ReadFromJsonAsync<SiteVerifyResponse>();

                if (result?.Success != true)
                    _logger.LogWarning("Turnstile recusou o envio: {Errors}", string.Join(", ", result?.ErrorCodes ?? []));

                return result?.Success == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar o token do Turnstile.");
                return false;
            }
        }

        private record SiteVerifyResponse(
            [property: JsonPropertyName("success")] bool Success,
            [property: JsonPropertyName("error-codes")] string[]? ErrorCodes);
    }
}
