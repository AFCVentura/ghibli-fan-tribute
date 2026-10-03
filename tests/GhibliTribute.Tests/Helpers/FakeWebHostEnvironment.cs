using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace GhibliTribute.Tests.Helpers
{
    /// <summary>
    ///    IWebHostEnvironment mínimo: nome do ambiente e um wwwroot apontando pra uma pasta qualquer (ou vazio).
    /// </summary>
    public class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public FakeWebHostEnvironment(string environmentName = "Production", string? webRootPath = null)
        {
            EnvironmentName = environmentName;
            WebRootPath = webRootPath ?? "";
            WebRootFileProvider = webRootPath is null ? new NullFileProvider() : new PhysicalFileProvider(webRootPath);
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "GhibliTribute";
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
