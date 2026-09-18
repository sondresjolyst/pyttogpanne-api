using FluentValidation;
using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Features.Recipes
{
    /// <summary>The categories a recipe is filed under. Public read, admin write.</summary>
    public static class RecipeCategoryEndpoints
    {
        public static async Task<IResult> GetAll(ApplicationDbContext db, CancellationToken ct)
        {
            var categories = await db.RecipeCategories
                .AsNoTracking()
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Key)
                .ToListAsync(ct);

            return TypedResults.Ok(categories.Select(RecipeMapping.ToDto));
        }

        public static async Task<IResult> Create(RecipeCategoryInput body, ApplicationDbContext db, CancellationToken ct)
        {
            var key = body.Key.Trim().ToLowerInvariant();
            if (await db.RecipeCategories.AnyAsync(c => c.Key == key, ct))
                return TypedResults.Problem("A category with that key already exists.", statusCode: StatusCodes.Status409Conflict);

            var category = new RecipeCategory { Key = key, Name = body.Name.Trim(), SortOrder = body.SortOrder };
            db.RecipeCategories.Add(category);
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok(RecipeMapping.ToDto(category));
        }

        public static async Task<IResult> Update(int id, RecipeCategoryInput body, ApplicationDbContext db, CancellationToken ct)
        {
            var category = await db.RecipeCategories.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (category == null) return TypedResults.NotFound();

            var key = body.Key.Trim().ToLowerInvariant();
            if (await db.RecipeCategories.AnyAsync(c => c.Id != id && c.Key == key, ct))
                return TypedResults.Problem("A category with that key already exists.", statusCode: StatusCodes.Status409Conflict);

            category.Key = key;
            category.Name = body.Name.Trim();
            category.SortOrder = body.SortOrder;
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok(RecipeMapping.ToDto(category));
        }

        /// <summary>Recipes keep their other fields; the category is simply cleared from them.</summary>
        public static async Task<IResult> Delete(int id, ApplicationDbContext db, CancellationToken ct)
        {
            var category = await db.RecipeCategories.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (category == null) return TypedResults.NotFound();

            db.RecipeCategories.Remove(category);
            await db.SaveChangesAsync(ct);
            return TypedResults.NoContent();
        }

        public class Endpoints : IEndpoint
        {
            public void Map(IEndpointRouteBuilder app)
            {
                var group = app.MapGroup("/api/recipe-categories");
                group.MapGet("", GetAll).AllowAnonymous();
                group.MapPost("", Create).RequireAuthorization(Policies.Admin).WithValidation<RecipeCategoryInput>();
                group.MapPut("{id:int}", Update).RequireAuthorization(Policies.Admin).WithValidation<RecipeCategoryInput>();
                group.MapDelete("{id:int}", Delete).RequireAuthorization(Policies.Admin);
            }
        }
    }

    public class RecipeCategoryInput
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class RecipeCategoryValidator : AbstractValidator<RecipeCategoryInput>
    {
        public RecipeCategoryValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty().MaximumLength(60)
                .Matches("^[a-z0-9-]+$").WithMessage("Key may only contain lower-case letters, digits and hyphens.");

            RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        }
    }
}
