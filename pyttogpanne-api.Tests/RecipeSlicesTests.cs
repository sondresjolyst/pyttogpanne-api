using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Features.Recipes;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Recipes;
using System.Security.Claims;
using Xunit;

namespace pyttogpanne_api.Tests;

public class RecipeSlicesTests : TestBase
{
    private static HttpContext Anonymous() => new DefaultHttpContext();

    private static HttpContext AsAdmin()
    {
        var claims = new ClaimsIdentity([new Claim(ClaimTypes.Role, RoleNames.Admin)], "Test");
        return new DefaultHttpContext { User = new ClaimsPrincipal(claims) };
    }

    private static RecipeInput Input(string title = "Fiskegrateng i panna", bool published = true) => new()
    {
        Title = title,
        Intro = "Ett panne, ein brennar.",
        Servings = 2,
        PrepMinutes = 10,
        CookMinutes = 20,
        Difficulty = "Enkel",
        IsPublished = published,
        Ingredients =
        [
            new RecipeIngredientInput { Amount = "400", Unit = "g", Name = "torsk" },
            new RecipeIngredientInput { GroupName = "Til sausen", Amount = "2", Unit = "dl", Name = "fløte" }
        ],
        Steps =
        [
            new RecipeStepInput { Text = "Varm panna." },
            new RecipeStepInput { Text = "Legg i fisken." }
        ]
    };

    private static async Task<Recipe> SeedRecipeAsync(ApplicationDbContext db, string slug = "turgrot", bool published = true)
    {
        var recipe = new Recipe
        {
            Slug = slug,
            Title = "Turgrøt",
            IsPublished = published,
            PublishedAt = published ? DateTime.UtcNow : null,
            Ingredients = [new RecipeIngredient { SortOrder = 0, Name = "havregryn" }],
            Steps = [new RecipeStep { SortOrder = 0, Text = "Kok." }]
        };
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return recipe;
    }

    [Fact]
    public async Task Create_StoresIngredientsAndStepsInOrder()
    {
        await using var db = CreateDbContext();

        var created = Assert.IsType<Created<RecipeDetailDto>>(await RecipeCommands.Create(Input(), db, default));

        Assert.Equal("fiskegrateng-i-panna", created.Value!.Slug);
        Assert.Equal(["torsk", "fløte"], created.Value.Ingredients.Select(i => i.Name));
        Assert.Equal([0, 1], created.Value.Steps.Select(s => s.SortOrder));
        Assert.Equal(30, created.Value.TotalMinutes);
        Assert.NotNull(created.Value.PublishedAt);
    }

    [Fact]
    public async Task Create_DraftHasNoPublishedDate()
    {
        await using var db = CreateDbContext();

        var created = Assert.IsType<Created<RecipeDetailDto>>(await RecipeCommands.Create(Input(published: false), db, default));

        Assert.Null(created.Value!.PublishedAt);
        Assert.False(created.Value.IsPublished);
    }

    [Fact]
    public async Task Create_SameTitleTwice_GetsDistinctSlugs()
    {
        await using var db = CreateDbContext();

        var first = Assert.IsType<Created<RecipeDetailDto>>(await RecipeCommands.Create(Input(), db, default));
        var second = Assert.IsType<Created<RecipeDetailDto>>(await RecipeCommands.Create(Input(), db, default));

        Assert.Equal("fiskegrateng-i-panna", first.Value!.Slug);
        Assert.Equal("fiskegrateng-i-panna-2", second.Value!.Slug);
    }

    [Fact]
    public async Task Create_UnknownCategory_Returns400()
    {
        await using var db = CreateDbContext();

        var input = Input();
        input.CategoryIds = [999];

        var problem = Assert.IsType<ProblemHttpResult>(await RecipeCommands.Create(input, db, default));
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
    }

    [Fact]
    public async Task Create_WithCategory_LinksIt()
    {
        await using var db = CreateDbContext();
        await SeedData.EnsureRecipeCategoriesAsync(db);
        var middag = await db.RecipeCategories.FirstAsync(c => c.Key == "middag");

        var input = Input();
        input.CategoryIds = [middag.Id];

        var created = Assert.IsType<Created<RecipeDetailDto>>(await RecipeCommands.Create(input, db, default));
        Assert.Equal("middag", Assert.Single(created.Value!.Categories).Key);
    }

    [Fact]
    public async Task Update_ReplacesChildrenRatherThanAppending()
    {
        await using var db = CreateDbContext();
        var recipe = await SeedRecipeAsync(db);

        var input = Input(title: "Turgrøt");
        var ok = Assert.IsType<Ok<RecipeDetailDto>>(await RecipeCommands.Update(recipe.Id, input, db, default));

        Assert.Equal(2, ok.Value!.Ingredients.Count);
        Assert.Equal(2, ok.Value.Steps.Count);
        Assert.Equal(2, await db.RecipeIngredients.CountAsync());
    }

    [Fact]
    public async Task Update_NewTitle_ReSlugsAndTombstonesTheOldSlug()
    {
        await using var db = CreateDbContext();
        var recipe = await SeedRecipeAsync(db, slug: "turgrot");

        var ok = Assert.IsType<Ok<RecipeDetailDto>>(await RecipeCommands.Update(recipe.Id, Input(title: "Fjellgrøt"), db, default));

        Assert.Equal("fjellgrot", ok.Value!.Slug);
        Assert.Equal("turgrot", (await db.DeletedRecipes.SingleAsync()).Slug);
    }

    [Fact]
    public async Task Update_Unpublishing_TombstonesTheSlug()
    {
        await using var db = CreateDbContext();
        var recipe = await SeedRecipeAsync(db, slug: "turgrot");

        await RecipeCommands.Update(recipe.Id, Input(title: "Turgrøt", published: false), db, default);

        Assert.Equal("turgrot", (await db.DeletedRecipes.SingleAsync()).Slug);
    }

    [Fact]
    public async Task Update_RePublishing_ClearsTheTombstone()
    {
        await using var db = CreateDbContext();
        var recipe = await SeedRecipeAsync(db, slug: "turgrot");

        await RecipeCommands.Update(recipe.Id, Input(title: "Turgrøt", published: false), db, default);
        await RecipeCommands.Update(recipe.Id, Input(title: "Turgrøt", published: true), db, default);

        Assert.Empty(await db.DeletedRecipes.ToListAsync());
    }

    [Fact]
    public async Task Update_Missing_Returns404()
    {
        await using var db = CreateDbContext();
        Assert.IsType<NotFound>(await RecipeCommands.Update(404, Input(), db, default));
    }

    [Fact]
    public async Task Delete_RemovesRecipeAndTombstonesTheSlug()
    {
        await using var db = CreateDbContext();
        var recipe = await SeedRecipeAsync(db, slug: "turgrot");

        Assert.IsType<NoContent>(await RecipeCommands.Delete(recipe.Id, db, default));

        Assert.Empty(await db.Recipes.ToListAsync());
        Assert.Equal("turgrot", (await db.DeletedRecipes.SingleAsync()).Slug);
    }

    [Fact]
    public async Task GetAll_HidesDraftsFromTheApp()
    {
        await using var db = CreateDbContext();
        await SeedRecipeAsync(db, slug: "publisert", published: true);
        await SeedRecipeAsync(db, slug: "kladd", published: false);

        var ok = Assert.IsType<Ok<IEnumerable<RecipeSummaryDto>>>(
            await RecipeQueries.GetAll(Anonymous(), db, default));

        Assert.Equal("publisert", Assert.Single(ok.Value!).Slug);
    }

    [Fact]
    public async Task GetAll_AdminWithAll_SeesDrafts()
    {
        await using var db = CreateDbContext();
        await SeedRecipeAsync(db, slug: "publisert", published: true);
        await SeedRecipeAsync(db, slug: "kladd", published: false);

        var ok = Assert.IsType<Ok<IEnumerable<RecipeSummaryDto>>>(
            await RecipeQueries.GetAll(AsAdmin(), db, default, all: true));

        Assert.Equal(2, ok.Value!.Count());
    }

    [Fact]
    public async Task GetAll_AnonymousAskingForAll_StillOnlySeesPublished()
    {
        await using var db = CreateDbContext();
        await SeedRecipeAsync(db, slug: "kladd", published: false);

        var ok = Assert.IsType<Ok<IEnumerable<RecipeSummaryDto>>>(
            await RecipeQueries.GetAll(Anonymous(), db, default, all: true));

        Assert.Empty(ok.Value!);
    }

    [Fact]
    public async Task GetAll_FiltersByCategory()
    {
        await using var db = CreateDbContext();
        await SeedData.EnsureRecipeCategoriesAsync(db);
        var middag = await db.RecipeCategories.FirstAsync(c => c.Key == "middag");

        var input = Input();
        input.CategoryIds = [middag.Id];
        await RecipeCommands.Create(input, db, default);
        await SeedRecipeAsync(db, slug: "utan-kategori");

        var ok = Assert.IsType<Ok<IEnumerable<RecipeSummaryDto>>>(
            await RecipeQueries.GetAll(Anonymous(), db, default, category: "middag"));

        Assert.Equal("fiskegrateng-i-panna", Assert.Single(ok.Value!).Slug);
    }

    [Fact]
    public async Task GetBySlug_DraftIsNotFoundForTheApp()
    {
        await using var db = CreateDbContext();
        await SeedRecipeAsync(db, slug: "kladd", published: false);

        Assert.IsType<NotFound>(await RecipeQueries.GetBySlug("kladd", Anonymous(), db, default));
    }

    [Fact]
    public async Task GetBySlug_DraftIsVisibleToAdmin()
    {
        await using var db = CreateDbContext();
        await SeedRecipeAsync(db, slug: "kladd", published: false);

        var ok = Assert.IsType<Ok<RecipeDetailDto>>(await RecipeQueries.GetBySlug("kladd", AsAdmin(), db, default));
        Assert.Equal("kladd", ok.Value!.Slug);
    }

    [Fact]
    public async Task GetSync_ReturnsPublishedRecipesWithFullDetail()
    {
        await using var db = CreateDbContext();
        await SeedRecipeAsync(db, slug: "turgrot");
        await SeedRecipeAsync(db, slug: "kladd", published: false);

        var ok = Assert.IsType<Ok<RecipeSyncDto>>(await RecipeQueries.GetSync(db, default));

        var recipe = Assert.Single(ok.Value!.Recipes);
        Assert.Equal("turgrot", recipe.Slug);
        Assert.NotEmpty(recipe.Ingredients);
        Assert.NotEmpty(recipe.Steps);
    }

    [Fact]
    public async Task GetSync_Since_ReturnsOnlyWhatChanged()
    {
        await using var db = CreateDbContext();
        var old = await SeedRecipeAsync(db, slug: "gammal");
        old.UpdatedAt = DateTime.UtcNow.AddDays(-2);
        await db.SaveChangesAsync();

        var cutoff = DateTime.UtcNow.AddDays(-1);
        await RecipeCommands.Create(Input(), db, default);

        var ok = Assert.IsType<Ok<RecipeSyncDto>>(await RecipeQueries.GetSync(db, default, since: cutoff));

        Assert.Equal("fiskegrateng-i-panna", Assert.Single(ok.Value!.Recipes).Slug);
    }

    [Fact]
    public async Task GetSync_Since_ReportsDeletedSlugs()
    {
        await using var db = CreateDbContext();
        var recipe = await SeedRecipeAsync(db, slug: "turgrot");
        var cutoff = DateTime.UtcNow.AddSeconds(-1);

        await RecipeCommands.Delete(recipe.Id, db, default);

        var ok = Assert.IsType<Ok<RecipeSyncDto>>(await RecipeQueries.GetSync(db, default, since: cutoff));

        Assert.Empty(ok.Value!.Recipes);
        Assert.Equal("turgrot", Assert.Single(ok.Value.DeletedSlugs));
    }
}
