using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Infrastructure;
using Xunit;

namespace pyttogpanne_api.Tests;

public class SeedDataTests : TestBase
{
    [Fact]
    public async Task RecipeCategories_AreSeededOnce()
    {
        await using var db = CreateDbContext();

        await SeedData.EnsureRecipeCategoriesAsync(db);
        var afterFirst = await db.RecipeCategories.CountAsync();

        await SeedData.EnsureRecipeCategoriesAsync(db);

        Assert.Equal(afterFirst, await db.RecipeCategories.CountAsync());
        Assert.NotEqual(0, afterFirst);

        var middag = await db.RecipeCategories.FirstAsync(c => c.Key == "middag");
        Assert.Equal("Middag", middag.Name);
    }

    [Fact]
    public void LegalSeedText_CoversEveryPageInEveryLocale()
    {
        foreach (var key in new[] { "terms", "privacy", "cookies" })
        {
            foreach (var locale in Locales.Supported)
            {
                var page = LegalSeedText.All.SingleOrDefault(p => p.Key == key && p.Locale == locale);
                Assert.NotNull(page);
                Assert.NotEmpty(page!.BodyMarkdown);
                Assert.NotEmpty(page.Title);
            }
        }
    }

}
