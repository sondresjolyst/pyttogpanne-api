using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Models.Recipes;
using Xunit;

namespace pyttogpanne_api.Tests;

/// <summary>The package's orphan cleanup over this app's model: each table that shows an image keeps it.</summary>
public class ImageCleanupTests : TestBase
{
    [Fact]
    public async Task DeleteOrphans_RemovesAnImageNothingPointsAt()
    {
        await using var db = CreateDbContext();
        using var images = new TestImages(db);
        db.ContentImages.Add(images.Stored("aaaa"));
        await db.SaveChangesAsync();

        await images.Service.DeleteOrphansAsync(["aaaa"]);

        Assert.Empty(await db.ContentImages.ToListAsync());
        Assert.False(images.FileExists("aaaa"));
    }

    [Fact]
    public async Task DeleteOrphans_KeepsAnImageAStepStillShows()
    {
        await using var db = CreateDbContext();
        using var images = new TestImages(db);
        db.ContentImages.Add(images.Stored("aaaa"));
        db.Recipes.Add(new Recipe
        {
            Slug = "med-stegbilde",
            Title = "Med stegbilde",
            Steps = [new RecipeStep { SortOrder = 0, Text = "Stek.", ContentImageId = "aaaa" }],
        });
        await db.SaveChangesAsync();

        await images.Service.DeleteOrphansAsync(["aaaa"]);

        Assert.Single(await db.ContentImages.ToListAsync());
        Assert.True(images.FileExists("aaaa"));
    }

    [Fact]
    public async Task DeleteOrphans_KeepsAnImageAGearItemStillShows()
    {
        await using var db = CreateDbContext();
        using var images = new TestImages(db);
        db.ContentImages.Add(images.Stored("aaaa"));
        db.GearItems.Add(new GearItem
        {
            Slug = "brennar",
            Title = "Brennar",
            Body = "Tekst",
            Images = [new GearImage { ContentImageId = "aaaa", SortOrder = 0 }],
        });
        await db.SaveChangesAsync();

        await images.Service.DeleteOrphansAsync(["aaaa"]);

        Assert.Single(await db.ContentImages.ToListAsync());
    }

    [Fact]
    public async Task DeleteOrphans_KeepsAnImageARecipeStillShows()
    {
        await using var db = CreateDbContext();
        using var images = new TestImages(db);
        db.ContentImages.Add(images.Stored("aaaa"));
        db.Recipes.Add(new Recipe
        {
            Slug = "med-bilde",
            Title = "Med bilde",
            Images = [new RecipeImage { ContentImageId = "aaaa", SortOrder = 0 }],
        });
        await db.SaveChangesAsync();

        await images.Service.DeleteOrphansAsync(["aaaa"]);

        Assert.Single(await db.ContentImages.ToListAsync());
    }
}
