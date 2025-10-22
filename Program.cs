using BlazorServerFirstProject.Components;
using BlazorServerFirstProject.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

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
        builder.Services.AddScoped<ImdbApiService>();

        var app = builder.Build();

        var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(locOptions);


        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
