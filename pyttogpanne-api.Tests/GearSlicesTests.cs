using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Features.Gear;
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
            await GearEndpoints.Update(created.Value!.Id, Input(title: "Gassbrennar"), db, default));

        Assert.Equal("gassbrennar", ok.Value!.Slug);
    }

    [Fact]
    public async Task Update_Missing_Returns404()
    {
        await using var db = CreateDbContext();
        Assert.IsType<NotFound>(await GearEndpoints.Update(404, Input(), db, default));
    }

    [Fact]
    public async Task Delete_RemovesTheItem()
    {
        await using var db = CreateDbContext();
        var created = Assert.IsType<Created<GearItemDto>>(await GearEndpoints.Create(Input(), db, default));

        Assert.IsType<NoContent>(await GearEndpoints.Delete(created.Value!.Id, db, default));
        Assert.IsType<NotFound>(await GearEndpoints.GetBySlug("stormkjokken", AsAdmin(), db, default));
    }
}
