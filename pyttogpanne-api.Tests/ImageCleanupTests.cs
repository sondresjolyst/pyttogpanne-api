using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Recipes;
using pyttogpanne_api.Services;
using Xunit;

namespace pyttogpanne_api.Tests;

public class ImageCleanupTests : TestBase
{
    private static ContentImage Image(string id) =>
        new() { Id = id, FileName = $"{id}.png", ContentType = "image/png", StoredPath = $"{id}.png" };

    [Fact]
    public async Task DeleteOrphans_RemovesAnImageNothingPointsAt()
    {
        await using var db = CreateDbContext();
        db.ContentImages.Add(Image("aaaa"));
        await db.SaveChangesAsync();

        var storage = new FakeImageStorage();
        await ImageCleanup.DeleteOrphansAsync(["aaaa"], db, storage, default);

        Assert.Empty(await db.ContentImages.ToListAsync());
        Assert.Equal(1, storage.DeleteCount);
    }

    [Fact]
    public async Task DeleteOrphans_KeepsAnImageAStepStillShows()
    {
        await using var db = CreateDbContext();
        db.ContentImages.Add(Image("aaaa"));
        db.Recipes.Add(new Recipe
        {
            Slug = "med-stegbilde",
            Title = "Med stegbilde",
            Steps = [new RecipeStep { SortOrder = 0, Text = "Stek.", ContentImageId = "aaaa" }],
        });
        await db.SaveChangesAsync();

        var storage = new FakeImageStorage();
        await ImageCleanup.DeleteOrphansAsync(["aaaa"], db, storage, default);

        Assert.Single(await db.ContentImages.ToListAsync());
        Assert.Equal(0, storage.DeleteCount);
    }

    [Fact]
    public async Task DeleteOrphans_KeepsAnImageAGearItemStillShows()
    {
        await using var db = CreateDbContext();
        db.ContentImages.Add(Image("aaaa"));
        db.GearItems.Add(new GearItem
        {
            Slug = "brennar",
            Title = "Brennar",
            Body = "Tekst",
            Images = [new GearImage { ContentImageId = "aaaa", SortOrder = 0 }],
        });
        await db.SaveChangesAsync();

        var storage = new FakeImageStorage();
        await ImageCleanup.DeleteOrphansAsync(["aaaa"], db, storage, default);

        Assert.Single(await db.ContentImages.ToListAsync());
    }

    [Fact]
    public async Task DeleteOrphans_IgnoresAnEmptyCandidateList()
    {
        await using var db = CreateDbContext();
        db.ContentImages.Add(Image("aaaa"));
        await db.SaveChangesAsync();

        await ImageCleanup.DeleteOrphansAsync([], db, new FakeImageStorage(), default);

        Assert.Single(await db.ContentImages.ToListAsync());
    }

    /// <summary>
    /// The cleanup checks three tables by hand. A new table pointing at ContentImages has to be
    /// added there too, or its images would be deleted while still in use; this fails first.
    /// </summary>
    [Fact]
    public async Task ContentImages_IsReferencedByExactlyTheTablesCleanupChecks()
    {
        await using var db = CreateDbContext();

        var referencing = db.Model
            .FindEntityType(typeof(ContentImage))!
            .GetReferencingForeignKeys()
            .Select(fk => fk.DeclaringEntityType.ClrType.Name)
            .OrderBy(name => name)
            .ToList();

        Assert.Equal(["ContentImageVariant", "GearImage", "RecipeImage", "RecipeStep"], referencing);
    }
}
