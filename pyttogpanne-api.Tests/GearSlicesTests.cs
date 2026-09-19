using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using pyttogpanne_api.Constants;
using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Features.Gear;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models;
using System.Security.Claims;
using Xunit;

namespace pyttogpanne_api.Tests;

public class GearSlicesTests : TestBase
{
    private static HttpContext Anonymous() => new DefaultHttpContext();

    private static HttpContext AsAdmin()
    {
        var claims = new ClaimsIdentity([new Claim(ClaimTypes.Role, RoleNames.Admin)], "Test");
        return new DefaultHttpContext { User = new ClaimsPrincipal(claims) };
    }

    private static GearItemInput Input(string title = "Stormkjøkken", string kind = "Utstyr", bool published = true) => new()
    {
        Title = title,
        Kind = kind,
        Summary = "Brennar som toler vind.",
        Body = "## Kva du treng\n\nEin brennar og ei djup panne.",
        IsPublished = published
    };

    [Fact]
    public async Task Create_SlugsTheTitle()
    {
        await using var db = CreateDbContext();

        var created = Assert.IsType<Created<GearItemDto>>(await GearEndpoints.Create(Input(), db, default));

        Assert.Equal("stormkjokken", created.Value!.Slug);
        Assert.Equal("Utstyr", created.Value.Kind);
    }

    [Fact]
    public async Task Create_SameTitleTwice_GetsDistinctSlugs()
    {
        await using var db = CreateDbContext();

        await GearEndpoints.Create(Input(), db, default);
        var second = Assert.IsType<Created<GearItemDto>>(await GearEndpoints.Create(Input(), db, default));

        Assert.Equal("stormkjokken-2", second.Value!.Slug);
    }

    [Fact]
    public async Task Create_TakesTheFirstPhotoAsTheCover()
    {
        await using var db = CreateDbContext();

        var input = Input();
        input.Images = [
            new GalleryImageInput { ContentImageId = "aaaa", Caption = "Brennaren i bruk" },
            new GalleryImageInput { ContentImageId = "bbbb" },
        ];

        var created = Assert.IsType<Created<GearItemDto>>(await GearEndpoints.Create(input, db, default));

        Assert.Equal(["aaaa", "bbbb"], created.Value!.Images.Select(i => i.ContentImageId));
        Assert.Equal("aaaa", created.Value.CoverImageId);
        Assert.Equal("Brennaren i bruk", created.Value.Images[0].Caption);
    }

    [Fact]
    public async Task Update_ReorderingThePhotosChangesTheCover()
    {
        await using var db = CreateDbContext();

        var input = Input();
        input.Images = [new GalleryImageInput { ContentImageId = "aaaa" }, new GalleryImageInput { ContentImageId = "bbbb" }];
        var created = Assert.IsType<Created<GearItemDto>>(await GearEndpoints.Create(input, db, default));

        input.Images = [new GalleryImageInput { ContentImageId = "bbbb" }, new GalleryImageInput { ContentImageId = "aaaa" }];
        var ok = Assert.IsType<Ok<GearItemDto>>(
            await GearEndpoints.Update(created.Value!.Id, input, db, new FakeImageStorage(), default));

        Assert.Equal("bbbb", ok.Value!.CoverImageId);
    }

    [Fact]
    public async Task Delete_RemovesTheStoredPhotos()
    {
        await using var db = CreateDbContext();
        db.ContentImages.Add(new ContentImage { Id = "aaaa", FileName = "a.png", ContentType = "image/png", StoredPath = "a.png" });
        await db.SaveChangesAsync();

        var input = Input();
        input.Images = [new GalleryImageInput { ContentImageId = "aaaa" }];
        var created = Assert.IsType<Created<GearItemDto>>(await GearEndpoints.Create(input, db, default));

        var storage = new FakeImageStorage();
        await GearEndpoints.Delete(created.Value!.Id, db, storage, default);

        Assert.Equal(1, storage.DeleteCount);
        Assert.Empty(await db.ContentImages.ToListAsync());
    }

    [Fact]
    public async Task GetAll_HidesDraftsFromTheApp()
    {
        await using var db = CreateDbContext();
        await GearEndpoints.Create(Input(title: "Publisert"), db, default);
        await GearEndpoints.Create(Input(title: "Kladd", published: false), db, default);

        var ok = Assert.IsType<Ok<IEnumerable<GearItemDto>>>(await GearEndpoints.GetAll(Anonymous(), db, default));

        Assert.Equal("publisert", Assert.Single(ok.Value!).Slug);
    }

    [Fact]
    public async Task GetAll_AdminWithAll_SeesDrafts()
    {
        await using var db = CreateDbContext();
        await GearEndpoints.Create(Input(title: "Publisert"), db, default);
        await GearEndpoints.Create(Input(title: "Kladd", published: false), db, default);

        var ok = Assert.IsType<Ok<IEnumerable<GearItemDto>>>(await GearEndpoints.GetAll(AsAdmin(), db, default, all: true));

        Assert.Equal(2, ok.Value!.Count());
    }

    [Fact]
    public async Task GetAll_FiltersByKind()
    {
        await using var db = CreateDbContext();
        await GearEndpoints.Create(Input(title: "Brennar", kind: "Utstyr"), db, default);
        await GearEndpoints.Create(Input(title: "Pakk lett", kind: "Tips"), db, default);

        var ok = Assert.IsType<Ok<IEnumerable<GearItemDto>>>(await GearEndpoints.GetAll(Anonymous(), db, default, kind: "tips"));

        Assert.Equal("pakk-lett", Assert.Single(ok.Value!).Slug);
    }

    [Fact]
    public async Task GetAll_UnknownKind_Returns400()
    {
        await using var db = CreateDbContext();

        var problem = Assert.IsType<ProblemHttpResult>(await GearEndpoints.GetAll(Anonymous(), db, default, kind: "bananas"));
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
    }

    [Fact]
    public async Task GetBySlug_DraftIsNotFoundForTheApp()
    {
        await using var db = CreateDbContext();
        await GearEndpoints.Create(Input(title: "Kladd", published: false), db, default);

        Assert.IsType<NotFound>(await GearEndpoints.GetBySlug("kladd", Anonymous(), db, default));
    }

    [Fact]
    public async Task Update_NewTitle_ReSlugs()
    {
        await using var db = CreateDbContext();
        var created = Assert.IsType<Created<GearItemDto>>(await GearEndpoints.Create(Input(), db, default));

        var ok = Assert.IsType<Ok<GearItemDto>>(
            await GearEndpoints.Update(created.Value!.Id, Input(title: "Gassbrennar"), db, new FakeImageStorage(), default));

        Assert.Equal("gassbrennar", ok.Value!.Slug);
    }

    [Fact]
    public async Task Update_Missing_Returns404()
    {
        await using var db = CreateDbContext();
        Assert.IsType<NotFound>(await GearEndpoints.Update(404, Input(), db, new FakeImageStorage(), default));
    }

    [Fact]
    public async Task Delete_RemovesTheItem()
    {
        await using var db = CreateDbContext();
        var created = Assert.IsType<Created<GearItemDto>>(await GearEndpoints.Create(Input(), db, default));

        Assert.IsType<NoContent>(await GearEndpoints.Delete(created.Value!.Id, db, new FakeImageStorage(), default));
        Assert.IsType<NotFound>(await GearEndpoints.GetBySlug("stormkjokken", AsAdmin(), db, default));
    }
}
