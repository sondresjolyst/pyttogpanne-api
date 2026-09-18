using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Features.Recipes;
using pyttogpanne_api.Infrastructure;
using Xunit;

namespace pyttogpanne_api.Tests;

public class RecipeCategorySlicesTests : TestBase
{
    private static RecipeCategoryInput Input(string key = "baal", string name = "Bålmat", int sortOrder = 100) =>
        new() { Key = key, Name = name, SortOrder = sortOrder };

    [Fact]
    public async Task GetAll_IsSortedBySortOrder()
    {
        await using var db = CreateDbContext();
        await SeedData.EnsureRecipeCategoriesAsync(db);

        var ok = Assert.IsType<Ok<IEnumerable<RecipeCategoryDto>>>(await RecipeCategoryEndpoints.GetAll(db, default));

        var keys = ok.Value!.Select(c => c.Key).ToList();
        Assert.Equal(["frokost", "lunsj", "middag"], keys.Take(3));
    }

    [Fact]
    public async Task Create_LowercasesTheKey()
    {
        await using var db = CreateDbContext();

        var ok = Assert.IsType<Ok<RecipeCategoryDto>>(await RecipeCategoryEndpoints.Create(Input(key: "BAAL"), db, default));

        Assert.Equal("baal", ok.Value!.Key);
    }

    [Fact]
    public async Task Create_DuplicateKey_Returns409()
    {
        await using var db = CreateDbContext();
        await RecipeCategoryEndpoints.Create(Input(), db, default);

        var problem = Assert.IsType<ProblemHttpResult>(await RecipeCategoryEndpoints.Create(Input(), db, default));
        Assert.Equal(StatusCodes.Status409Conflict, problem.StatusCode);
    }

    [Fact]
    public async Task Update_TakingAnotherKey_Returns409()
    {
        await using var db = CreateDbContext();
        await RecipeCategoryEndpoints.Create(Input(key: "baal"), db, default);
        var second = Assert.IsType<Ok<RecipeCategoryDto>>(await RecipeCategoryEndpoints.Create(Input(key: "sopp"), db, default));

        var problem = Assert.IsType<ProblemHttpResult>(
            await RecipeCategoryEndpoints.Update(second.Value!.Id, Input(key: "baal"), db, default));
        Assert.Equal(StatusCodes.Status409Conflict, problem.StatusCode);
    }

    [Fact]
    public async Task Update_Missing_Returns404()
    {
        await using var db = CreateDbContext();
        Assert.IsType<NotFound>(await RecipeCategoryEndpoints.Update(404, Input(), db, default));
    }

    [Fact]
    public async Task Delete_LeavesTheRecipeInPlace()
    {
        await using var db = CreateDbContext();
        var category = Assert.IsType<Ok<RecipeCategoryDto>>(await RecipeCategoryEndpoints.Create(Input(), db, default));

        var recipe = new RecipeInput
        {
            Title = "Bålkaffi",
            Difficulty = "Enkel",
            IsPublished = true,
            CategoryIds = [category.Value!.Id],
            Ingredients = [new RecipeIngredientInput { Name = "kaffi" }],
            Steps = [new RecipeStepInput { Text = "Kok." }]
        };
        await RecipeCommands.Create(recipe, db, default);

        Assert.IsType<NoContent>(await RecipeCategoryEndpoints.Delete(category.Value.Id, db, default));

        Assert.Single(await db.Recipes.ToListAsync());
        Assert.Empty(await db.RecipeCategoryLinks.ToListAsync());
    }
}
