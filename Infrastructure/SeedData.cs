using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Infrastructure
{
    /// <summary>Seeds the categories a recipe is filed under. Existing keys are left alone.</summary>
    public static class SeedData
    {
        private static readonly (string Key, int SortOrder, string Name)[] Categories =
        [
            ("frokost", 10, "Frokost"),
            ("lunsj", 20, "Lunsj"),
            ("middag", 30, "Middag"),
            ("dessert", 40, "Dessert"),
            ("snacks", 50, "Snacks"),
            ("fisk", 60, "Fisk"),
            ("kjott", 70, "Kjøtt"),
            ("vegetar", 80, "Vegetar"),
            ("bakst", 90, "Bakst")
        ];

        public static async Task EnsureRecipeCategoriesAsync(ApplicationDbContext db, CancellationToken ct = default)
        {
            var existing = await db.RecipeCategories.Select(c => c.Key).ToListAsync(ct);
            var missing = Categories.Where(c => !existing.Contains(c.Key)).ToList();
            if (missing.Count == 0) return;

            foreach (var (key, sortOrder, name) in missing)
            {
                db.RecipeCategories.Add(new RecipeCategory
                {
                    Key = key,
                    Name = name,
                    SortOrder = sortOrder
                });
            }

            await db.SaveChangesAsync(ct);
        }

    }
}
