using System.Net;
using System.Text;

namespace GhibliTribute.Tests.Helpers
{
    /// <summary>
    ///    Requisição que chegou no handler, com o corpo já lido (o HttpContent original é descartado depois do envio).
    /// </summary>
    public record CapturedRequest(HttpMethod Method, Uri Uri, string? Body, string? Authorization);

    /// <summary>
    ///    Substitui a rede nos testes: o HttpClient do service fala com este handler, que responde o que o teste mandar
    ///    e guarda cada requisição pra conferir depois. Feito à mão pra não depender de biblioteca de mock.
    /// </summary>
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

        public List<CapturedRequest> Requests { get; } = new();

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            _respond = respond;
        }

        // Atalho pro caso comum: sempre a mesma resposta JSON
        public static FakeHttpMessageHandler Json(string json, HttpStatusCode status = HttpStatusCode.OK) =>
            new(_ => JsonResponse(json, status));

        // Simula falha de rede (DNS, timeout, conexão recusada)
        public static FakeHttpMessageHandler Throws() =>
            new(_ => throw new HttpRequestException("rede fora do ar"));

        public static HttpResponseMessage JsonResponse(string json, HttpStatusCode status = HttpStatusCode.OK) =>
            new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

        public HttpClient CreateClient(string baseAddress) => new(this) { BaseAddress = new Uri(baseAddress) };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            Requests.Add(new CapturedRequest(request.Method, request.RequestUri!, body, request.Headers.Authorization?.ToString()));
            return _respond(request);
        }
    }
}
