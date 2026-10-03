using Microsoft.Extensions.Configuration;

namespace GhibliTribute.Tests.Helpers
{
    /// <summary>
    ///    IConfiguration em memória, no lugar do appsettings/user-secrets.
    /// </summary>
    public static class TestConfig
    {
        public static IConfiguration Build(params (string Key, string? Value)[] values) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(values.Select(v => new KeyValuePair<string, string?>(v.Key, v.Value)))
                .Build();
    }
}
