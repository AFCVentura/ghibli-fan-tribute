using BlazorServerFirstProject.Components;
using BlazorServerFirstProject.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Threading.RateLimiting;

namespace BlazorServerFirstProject;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Ativa uso de localização
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        // Define as culturas suportadas
        var supportedCultures = new[] 
        { 
            new CultureInfo("en-US"), 
            new CultureInfo("pt-BR") 
        };

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
            options.SupportedCultures = supportedCultures.ToList();
            options.SupportedUICultures = supportedCultures.ToList();
            options.RequestCultureProviders = new[]
            {
                new CookieRequestCultureProvider()
            };
        });
        builder.Services.AddScoped<CultureService>();
        builder.Services.AddScoped<ThemeService>();
        builder.Services.AddMemoryCache();
        // HttpClient gerenciado pela fábrica (em vez de new HttpClient() no service), já apontando pra OMDb
        builder.Services.AddHttpClient<ImdbApiService>(client =>
        {
            client.BaseAddress = new Uri("https://www.omdbapi.com/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        builder.Services.AddHttpClient<GhibliApiService>(client =>
        {
            client.BaseAddress = new Uri("https://ghibliapi.vercel.app/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        builder.Services.AddScoped<FilmCatalogService>();
        builder.Services.AddSingleton<AnalyticsService>();

        // Formulário de contato: Turnstile (anti-robô da Cloudflare) e envio de e-mail pela API do Resend
        builder.Services.AddHttpClient<TurnstileService>(client =>
        {
            client.BaseAddress = new Uri("https://challenges.cloudflare.com/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        builder.Services.AddHttpClient<ContactEmailService>(client =>
        {
            client.BaseAddress = new Uri("https://api.resend.com/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Limite de envios do formulário de contato por IP. Só vale pro POST do /contact; o resto do site
        // não tem limite. O rate limiter do ASP.NET só enxerga requisições HTTP, e por isso a página de
        // contato é estática (SSR) e não interativa. Em produção atrás de proxy (Azure Container Apps),
        // ASPNETCORE_FORWARDEDHEADERS_ENABLED=true faz o RemoteIpAddress ser o IP real do visitante.
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                HttpMethods.IsPost(context.Request.Method) && context.Request.Path.Equals("/contact", StringComparison.OrdinalIgnoreCase)
                    ? RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(10) })
                    : RateLimitPartition.GetNoLimiter("none"));

            // Volta pra página com um aviso, em vez de um 429 seco
            options.OnRejected = (context, _) =>
            {
                context.HttpContext.Response.Redirect("/contact?status=too-many");
                return ValueTask.CompletedTask;
            };
        });

        var app = builder.Build();

        var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(locOptions);


        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        // URL inexistente (ou página ainda não criada) cai na página 404 do site em vez de uma tela em branco
        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

        app.UseHttpsRedirection();

        app.UseRateLimiter();

        // Antiforgery inválido no formulário de contato (normalmente o container reiniciou entre abrir a página e
        // enviar, e as chaves mudaram) volta pra página com um aviso, em vez do erro 400 em texto puro
        app.Use(async (context, next) =>
        {
            if (HttpMethods.IsPost(context.Request.Method) && context.Request.Path.Equals("/contact", StringComparison.OrdinalIgnoreCase)
                && !await context.RequestServices.GetRequiredService<IAntiforgery>().IsRequestValidAsync(context))
            {
                context.Response.Redirect("/contact?status=expired");
                return;
            }
            await next();
        });

        app.UseAntiforgery();

        // Arquivos de wwwroot com impressão digital no nome (@Assets no App.razor): o navegador guarda em cache
        // pra sempre e baixa de novo assim que o conteúdo muda, sem ficar preso numa versão antiga
        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
