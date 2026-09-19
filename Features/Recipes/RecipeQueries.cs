using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Features.Recipes
{
    /// <summary>Public read endpoints for recipes (list, by slug) plus the admin editor's full view.</summary>
    public static class RecipeQueries
    {
        private static bool IsAdmin(HttpContext http) => http.User.IsInRole(RoleNames.Admin);

        private static IQueryable<Recipe> WithGraph(ApplicationDbContext db) =>
            db.Recipes
                .AsNoTracking()
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Images)
                .Include(r => r.Categories).ThenInclude(l => l.RecipeCategory)
                // Four collections in one query multiply out; split keeps each one flat.
                .AsSplitQuery();

        public static async Task<IResult> GetAll(HttpContext http, ApplicationDbContext db, CancellationToken ct,
            string? category = null, string? search = null, bool all = false)
        {
            var includeDrafts = all && IsAdmin(http);

            var query = db.Recipes
                .AsNoTracking()
                .Include(r => r.Images)
                .Include(r => r.Categories).ThenInclude(l => l.RecipeCategory)
                .AsSplitQuery()
                .AsQueryable();

            if (!includeDrafts)
                query = query.Where(r => r.IsPublished);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(r => r.Categories.Any(l => l.RecipeCategory!.Key == category));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = $"%{search.Trim()}%";
                query = query.Where(r =>
                    EF.Functions.ILike(r.Title, term) ||
                    (r.Intro != null && EF.Functions.ILike(r.Intro, term)) ||
                    r.Ingredients.Any(i => EF.Functions.ILike(i.Name, term)));
            }

            var recipes = await query
                .OrderByDescending(r => r.PublishedAt ?? r.CreatedAt)
                .ToListAsync(ct);

            return TypedResults.Ok(recipes.Select(RecipeMapping.ToSummary));
        }

        public static async Task<IResult> GetBySlug(string slug, HttpContext http, ApplicationDbContext db, CancellationToken ct)
        {
            var recipe = await WithGraph(db).FirstOrDefaultAsync(r => r.Slug == slug, ct);
            if (recipe == null || (!recipe.IsPublished && !IsAdmin(http)))
                return TypedResults.NotFound();

            return TypedResults.Ok(RecipeMapping.ToDetail(recipe));
        }

        public static async Task<IResult> GetForEdit(int id, ApplicationDbContext db, CancellationToken ct)
        {
            var recipe = await WithGraph(db).FirstOrDefaultAsync(r => r.Id == id, ct);
            if (recipe == null) return TypedResults.NotFound();
            return TypedResults.Ok(RecipeMapping.ToDetail(recipe));
        }

        /// <summary>
        /// Everything the app needs for offline use in one response: every published recipe with
        /// its full ingredient and step list, so a phone can sync before losing signal.
        /// </summary>
        public static async Task<IResult> GetSync(ApplicationDbContext db, CancellationToken ct, DateTime? since = null)
        {
            var query = WithGraph(db).Where(r => r.IsPublished);
            if (since.HasValue)
                query = query.Where(r => r.UpdatedAt > since.Value);

            var recipes = await query.OrderBy(r => r.Id).ToListAsync(ct);

            var tombstones = db.DeletedRecipes.AsNoTracking().AsQueryable();
            if (since.HasValue)
                tombstones = tombstones.Where(t => t.DeletedAt > since.Value);

            return TypedResults.Ok(new RecipeSyncDto
            {
                ServerTime = DateTime.UtcNow,
                Recipes = [.. recipes.Select(RecipeMapping.ToDetail)],
                DeletedSlugs = await tombstones.Select(t => t.Slug).ToListAsync(ct)
            });
        }

        public class Endpoints : IEndpoint
        {
            public void Map(IEndpointRouteBuilder app)
            {
                var group = app.MapGroup("/api/recipes").AllowAnonymous();
                group.MapGet("", GetAll);
                group.MapGet("sync", GetSync);
                group.MapGet("{id:int}/edit", GetForEdit).RequireAuthorization(Policies.Admin);
                group.MapGet("{slug}", GetBySlug);
            }
        }
    }

    public class RecipeSyncDto
    {
        public DateTime ServerTime { get; set; }
        public List<RecipeDetailDto> Recipes { get; set; } = [];

        /// <summary>Slugs removed since the client's last sync, so a cached copy can be dropped.</summary>
        public List<string> DeletedSlugs { get; set; } = [];
    }
}
