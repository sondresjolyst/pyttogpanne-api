using Microsoft.Extensions.Configuration;
using pyttogpanne_api.Services;
using Xunit;

namespace pyttogpanne_api.Tests;

public class SiteLinksTests
{
    private static IConfiguration Config(string? baseUrl) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Site:BaseUrl"] = baseUrl })
            .Build();

    [Fact]
    public void SetPassword_PointsAtTheConfiguredEnvironment()
    {
        var link = SiteLinks.SetPassword(Config("https://dev.pyttogpanne.no"), "ny@pyttogpanne.no", "YB71WN");

        Assert.Equal("https://dev.pyttogpanne.no/no/reset-password?email=ny%40pyttogpanne.no&code=YB71WN", link);
    }

    [Fact]
    public void SetPassword_TrimsATrailingSlash()
    {
        var link = SiteLinks.SetPassword(Config("https://www.pyttogpanne.no/"), "a@b.no", "ABC123");

        Assert.StartsWith("https://www.pyttogpanne.no/no/reset-password?", link);
    }

    [Fact]
    public void SetPassword_FallsBackToProductionWhenUnset()
    {
        var link = SiteLinks.SetPassword(Config(null), "a@b.no", "ABC123");

        Assert.StartsWith("https://www.pyttogpanne.no/no/reset-password?", link);
    }
}
