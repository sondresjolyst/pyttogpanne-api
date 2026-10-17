using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using pyttogpanne_api.Features.Admin;
using pyttogpanne_api.Models.Admin;
using pyttogpanne_api.Models.Recipes;
using SjolystInnovation.AppKit.Storage;
using SjolystInnovation.AppKit.Storage.Images;
using Xunit;

namespace pyttogpanne_api.Tests;

public class AdminSlicesTests : TestBase
{
    private static Recipe Dish(string slug, bool published) => new() { Slug = slug, Title = slug, IsPublished = published };

    [Fact]
    public async Task GetStats_ReturnsCounts()
    {
        await using var db = CreateDbContext();
        var deleted = MakeUser("u-del", "del@example.com");
        deleted.IsDeleted = true;
        db.Users.AddRange(MakeUser("u1", "a@example.com"), MakeUser("u2", "b@example.com"), deleted);
        db.Recipes.AddRange(Dish("a", true), Dish("b", true), Dish("c", false));
        db.ContentImages.Add(new ContentImage { FileName = "x.png", ContentType = "image/png", StoredPath = "x.png" });
        await db.SaveChangesAsync();

        var storage = new Mock<IAppKitStorageUsage>();
        storage.Setup(s => s.GetAsync(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { AppKitContentImages.StoreName })), default))
            .ReturnsAsync(new AppKitStorageUsage(1234, 5000, 4000));

        var ok = Assert.IsType<Ok<AdminStatsDto>>(await AdminDashboard.GetStats(db, storage.Object, default));
        Assert.Equal(2, ok.Value!.TotalUsers);
        Assert.Equal(2, ok.Value.PublishedRecipes);
        Assert.Equal(1, ok.Value.DraftRecipes);
        Assert.Equal(1, ok.Value.ContentImages);
        Assert.Equal(1234, ok.Value.StorageUsedBytes);
        Assert.Equal(5000, ok.Value.DiskTotalBytes);
        Assert.Equal(4000, ok.Value.DiskFreeBytes);
    }

    [Fact]
    public async Task SampleAsync_CountsTheLiveTotals()
    {
        await using var db = CreateDbContext();
        var deleted = MakeUser("u-del", "del@example.com");
        deleted.IsDeleted = true;
        db.Users.AddRange(MakeUser("u1", "a@example.com"), deleted);
        db.Recipes.AddRange(Dish("a", true), Dish("b", false), Dish("c", false));
        db.GearItems.Add(new GearItem { Slug = "brennar", Title = "Brennar", Body = "Tekst" });
        db.ContentImages.Add(new ContentImage { FileName = "x.png", ContentType = "image/png", StoredPath = "x.png" });
        await db.SaveChangesAsync();

        var sample = await AdminDashboard.SampleAsync(db, default);

        Assert.Equal(1, sample.TotalUsers);
        Assert.Equal(1, sample.PublishedRecipes);
        Assert.Equal(2, sample.DraftRecipes);
        Assert.Equal(1, sample.GearItemCount);
        Assert.Equal(1, sample.ContentImages);
    }

    /// <summary>The history's JSON keys come from these names, and the admin chart reads those keys.</summary>
    [Fact]
    public void DailyStatSnapshot_KeepsTheCounterNamesTheChartReads()
    {
        var counters = typeof(DailyStatSnapshot).GetProperties()
            .Where(p => p.PropertyType == typeof(int) && p.Name != nameof(DailyStatSnapshot.Id))
            .Select(p => p.Name);

        Assert.Equal(["TotalUsers", "PublishedRecipes", "DraftRecipes", "GearItemCount", "ContentImages"], counters);
    }
}
