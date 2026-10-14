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
        var link = SiteLinks.SetPasswordPage(Config("https://dev.pyttogpanne.no"));

        Assert.Equal("https://dev.pyttogpanne.no/no/reset-password", link);
    }

    [Fact]
    public void SetPassword_TrimsATrailingSlash()
    {
        var link = SiteLinks.SetPasswordPage(Config("https://www.pyttogpanne.no/"));

        Assert.Equal("https://www.pyttogpanne.no/no/reset-password", link);
    }

    [Fact]
    public void SetPassword_FallsBackToProductionWhenUnset()
    {
        var link = SiteLinks.SetPasswordPage(Config(null));

        Assert.Equal("https://www.pyttogpanne.no/no/reset-password", link);
    }
}
