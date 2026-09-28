using System.Net.Http.Headers;

namespace BlazorServerFirstProject.Services
{
    /// <summary>
    ///    Mensagem já validada do formulário de contato.
    /// </summary>
    public record ContactMessage(string Name, string Email, string Subject, string Message);

    /// <summary>
    ///    Envia a mensagem do formulário de contato por e-mail usando a API HTTP do Resend
    ///    (o Azure bloqueia SMTP na porta 25, e a API é mais simples de qualquer jeito).
    ///    O destinatário é sempre o dono do site ("Contact:ToEmail"), e o e-mail do visitante vai só no Reply-To.
    ///    Assim ninguém consegue usar o formulário pra mandar e-mail pra terceiros.
    ///    Sem "Resend:ApiKey" no desenvolvimento, a mensagem só aparece no log, pra dar pra testar sem conta.
    /// </summary>
    public class ContactEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContactEmailService> _logger;
        private readonly IHostEnvironment _environment;
        private readonly string? _apiKey;
        private readonly string? _from;
        private readonly string? _to;

        public ContactEmailService(HttpClient httpClient, IConfiguration configuration, IHostEnvironment environment, ILogger<ContactEmailService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _environment = environment;
            _apiKey = configuration["Resend:ApiKey"];
            _from = configuration["Resend:From"];
            _to = configuration["Contact:ToEmail"];
        }

        public async Task<bool> SendAsync(ContactMessage message)
        {
            var subject = $"[Ghibli Fan Tribute] {message.Subject} | {message.Name}";
            var body = $"""
                Nome: {message.Name}
                E-mail: {message.Email}
                Assunto: {message.Subject}

                {message.Message}
                """;

            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_to) || string.IsNullOrWhiteSpace(_from))
            {
                if (_environment.IsDevelopment())
                {
                    _logger.LogInformation("Resend não configurado, e-mail de contato só no log:\n{Subject}\n{Body}", subject, body);
                    return true;
                }

                _logger.LogError("Resend:ApiKey, Resend:From ou Contact:ToEmail não configurado, mensagem de contato perdida.");
                return false;
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
                {
                    // Só texto puro: nada do que o visitante digitou vira HTML no e-mail
                    Content = JsonContent.Create(new
                    {
                        from = _from,
                        to = new[] { _to },
                        reply_to = message.Email,
                        subject,
                        text = body
                    })
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Resend recusou o e-mail ({Status}): {Body}", (int)response.StatusCode, await response.Content.ReadAsStringAsync());
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar o e-mail de contato pelo Resend.");
                return false;
            }
        }
    }
}
