using pyttogpanne_api.Constants;
using Xunit;

namespace pyttogpanne_api.Tests;

public class LocalesTests
{
    [Theory]
    [InlineData("no", "no")]
    [InlineData("NO", "no")]
    [InlineData("nb", "no")]
    [InlineData("nb-NO", "no")]
    [InlineData("nn", "no")]
    [InlineData("en", "no")]
    [InlineData("de", "no")]
    [InlineData("", "no")]
    [InlineData(null, "no")]
    public void Normalize_MapsToSupportedLocale(string? input, string expected) =>
        Assert.Equal(expected, Locales.Normalize(input));

    [Theory]
    [InlineData("no", true)]
    [InlineData("en", false)]
    [InlineData("nb", false)]
    [InlineData(null, false)]
    public void IsSupported_OnlyAcceptsExactTags(string? input, bool expected) =>
        Assert.Equal(expected, Locales.IsSupported(input));
}
