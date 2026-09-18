using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Helpers;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Features.Recipes
{
    /// <summary>Admin create / update / delete for recipes, including their ingredients and steps.</summary>
    public static class RecipeCommands
    {
        public static async Task<IResult> Create(RecipeInput dto, ApplicationDbContext db, CancellationToken ct)
        {
            if (await UnknownCategoryAsync(dto.CategoryIds, db, ct))
                return TypedResults.Problem("One of those categories does not exist.", statusCode: StatusCodes.Status400BadRequest);

            var recipe = new Recipe
            {
                Slug = await UniqueSlugAsync(dto.Title, null, db, ct),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            ApplyScalars(recipe, dto);
            ApplyChildren(recipe, dto);

            if (recipe.IsPublished)
                recipe.PublishedAt = DateTime.UtcNow;

            db.Recipes.Add(recipe);
            await ClearTombstoneAsync(recipe.Slug, db, ct);
            await db.SaveChangesAsync(ct);

            var created = await LoadAsync(recipe.Id, db, ct);
            return TypedResults.Created($"/api/recipes/{recipe.Slug}", RecipeMapping.ToDetail(created!));
        }

        public static async Task<IResult> Update(int id, RecipeInput dto, ApplicationDbContext db, CancellationToken ct)
        {
            var recipe = await db.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
            if (recipe == null) return TypedResults.NotFound();

            if (await UnknownCategoryAsync(dto.CategoryIds, db, ct))
                return TypedResults.Problem("One of those categories does not exist.", statusCode: StatusCodes.Status400BadRequest);

            var wasPublished = recipe.IsPublished;
            var oldSlug = recipe.Slug;

            if (!string.Equals(recipe.Title, dto.Title.Trim(), StringComparison.Ordinal))
                recipe.Slug = await UniqueSlugAsync(dto.Title, recipe.Id, db, ct);

            ApplyScalars(recipe, dto);
            recipe.UpdatedAt = DateTime.UtcNow;

            if (recipe.IsPublished && !wasPublished)
                recipe.PublishedAt ??= DateTime.UtcNow;

            db.RecipeIngredients.RemoveRange(recipe.Ingredients);
            recipe.Ingredients.Clear();
            db.RecipeSteps.RemoveRange(recipe.Steps);
            recipe.Steps.Clear();
            db.RecipeCategoryLinks.RemoveRange(recipe.Categories);
            recipe.Categories.Clear();
            ApplyChildren(recipe, dto);

            if (!string.Equals(oldSlug, recipe.Slug, StringComparison.Ordinal))
                await AddTombstoneAsync(oldSlug, db, ct);
            if (wasPublished && !recipe.IsPublished)
                await AddTombstoneAsync(recipe.Slug, db, ct);
            if (recipe.IsPublished)
                await ClearTombstoneAsync(recipe.Slug, db, ct);

            await db.SaveChangesAsync(ct);

            var updated = await LoadAsync(recipe.Id, db, ct);
            return TypedResults.Ok(RecipeMapping.ToDetail(updated!));
        }

        public static async Task<IResult> Delete(int id, ApplicationDbContext db, CancellationToken ct)
        {
            var recipe = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (recipe == null) return TypedResults.NotFound();

            await AddTombstoneAsync(recipe.Slug, db, ct);
            db.Recipes.Remove(recipe);
            await db.SaveChangesAsync(ct);
            return TypedResults.NoContent();
        }

        private static void ApplyScalars(Recipe recipe, RecipeInput dto)
        {
            recipe.Title = dto.Title.Trim();
            recipe.Intro = Trimmed(dto.Intro);
            recipe.Servings = dto.Servings;
            recipe.PrepMinutes = dto.PrepMinutes;
            recipe.CookMinutes = dto.CookMinutes;
            recipe.Difficulty = Enum.Parse<RecipeDifficulty>(dto.Difficulty, ignoreCase: true);
            recipe.Tips = Trimmed(dto.Tips);
            recipe.CoverImageId = Trimmed(dto.CoverImageId);
            recipe.IsPublished = dto.IsPublished;
        }

        private static void ApplyChildren(Recipe recipe, RecipeInput dto)
        {
            var order = 0;
            foreach (var i in dto.Ingredients)
            {
                recipe.Ingredients.Add(new RecipeIngredient
                {
                    SortOrder = order++,
                    GroupName = Trimmed(i.GroupName),
                    Amount = Trimmed(i.Amount),
                    Unit = Trimmed(i.Unit),
                    Name = i.Name.Trim(),
                    Note = Trimmed(i.Note)
                });
            }

            order = 0;
            foreach (var s in dto.Steps)
            {
                recipe.Steps.Add(new RecipeStep
                {
                    SortOrder = order++,
                    Text = s.Text.Trim(),
                    ContentImageId = Trimmed(s.ContentImageId)
                });
            }

            foreach (var categoryId in dto.CategoryIds.Distinct())
                recipe.Categories.Add(new RecipeCategoryLink { RecipeCategoryId = categoryId });
        }

        private static async Task<bool> UnknownCategoryAsync(List<int> ids, ApplicationDbContext db, CancellationToken ct)
        {
            if (ids.Count == 0) return false;
            var known = await db.RecipeCategories.CountAsync(c => ids.Contains(c.Id), ct);
            return known != ids.Distinct().Count();
        }

        private static async Task AddTombstoneAsync(string slug, ApplicationDbContext db, CancellationToken ct)
        {
            var existing = await db.DeletedRecipes.FirstOrDefaultAsync(t => t.Slug == slug, ct);
            if (existing != null)
            {
                existing.DeletedAt = DateTime.UtcNow;
                return;
            }
            db.DeletedRecipes.Add(new DeletedRecipe { Slug = slug });
        }

        private static async Task ClearTombstoneAsync(string slug, ApplicationDbContext db, CancellationToken ct)
        {
            var existing = await db.DeletedRecipes.FirstOrDefaultAsync(t => t.Slug == slug, ct);
            if (existing != null) db.DeletedRecipes.Remove(existing);
        }

        private static async Task<Recipe?> LoadAsync(int id, ApplicationDbContext db, CancellationToken ct) =>
            await db.Recipes
                .AsNoTracking()
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Categories).ThenInclude(l => l.RecipeCategory)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

        private static async Task<string> UniqueSlugAsync(string title, int? excludeId, ApplicationDbContext db, CancellationToken ct)
        {
            var baseSlug = Slugify.Create(title);
            if (string.IsNullOrEmpty(baseSlug)) baseSlug = "oppskrift";

            var slug = baseSlug;
            var suffix = 2;
            while (await db.Recipes.AnyAsync(r => r.Slug == slug && r.Id != excludeId, ct))
            {
                slug = $"{baseSlug}-{suffix}";
                suffix++;
            }
            return slug;
        }

        private static string? Trimmed(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        public class Endpoints : IEndpoint
        {
            public void Map(IEndpointRouteBuilder app)
            {
                var group = app.MapGroup("/api/recipes").RequireAuthorization(Policies.Admin);
                group.MapPost("", Create).WithValidation<RecipeInput>();
                group.MapPut("{id:int}", Update).WithValidation<RecipeInput>();
                group.MapDelete("{id:int}", Delete);
            }
        }
    }
}
