using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models.Content;
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
    public async Task LegalPages_AreSeededForEveryKeyAndLocale()
    {
        await using var db = CreateDbContext();

        await SeedData.EnsureLegalPagesAsync(db);

        foreach (var key in LegalPageKeys.All)
        {
            foreach (var locale in Locales.Supported)
            {
                var page = await db.LegalPages.FirstOrDefaultAsync(p => p.Key == key && p.Locale == locale);
                Assert.NotNull(page);
                Assert.NotEmpty(page!.BodyMarkdown);
            }
        }
    }

    [Fact]
    public async Task LegalPages_DoNotOverwriteEditedText()
    {
        await using var db = CreateDbContext();
        db.LegalPages.Add(new LegalPage { Key = "terms", Locale = "no", Title = "Mine vilkår", BodyMarkdown = "Egen tekst" });
        await db.SaveChangesAsync();

        await SeedData.EnsureLegalPagesAsync(db);

        var page = await db.LegalPages.SingleAsync(p => p.Key == "terms" && p.Locale == "no");
        Assert.Equal("Egen tekst", page.BodyMarkdown);
    }
}
