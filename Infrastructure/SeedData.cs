using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Content;
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

        /// <summary>Writes the starting legal text for any (key, locale) that has none. Edited pages are left alone.</summary>
        public static async Task EnsureLegalPagesAsync(ApplicationDbContext db, CancellationToken ct = default)
        {
            var existing = await db.LegalPages.Select(p => new { p.Key, p.Locale }).ToListAsync(ct);
            var missing = LegalSeedText.All
                .Where(p => !existing.Any(e => e.Key == p.Key && e.Locale == p.Locale))
                .ToList();
            if (missing.Count == 0) return;

            foreach (var page in missing)
            {
                db.LegalPages.Add(new LegalPage
                {
                    Key = page.Key,
                    Locale = page.Locale,
                    Title = page.Title,
                    BodyMarkdown = page.Body
                });
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
