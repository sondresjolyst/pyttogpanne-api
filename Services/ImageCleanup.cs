using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Models;

namespace pyttogpanne_api.Services
{
    /// <summary>
    /// Deletes uploaded images that nothing points at any more. Call it after the rows that
    /// referenced them are saved, so every remaining reference is a real one.
    /// </summary>
    public static class ImageCleanup
    {
        public static async Task DeleteOrphansAsync(
            IReadOnlyCollection<string> candidateIds,
            ApplicationDbContext db,
            IImageStorageService images,
            CancellationToken ct)
        {
            if (candidateIds.Count == 0) return;

            var ids = candidateIds.Distinct().ToList();

            var orphans = await db.ContentImages
                .Include(i => i.Variants)
                .Where(i => ids.Contains(i.Id)
                    && !db.RecipeImages.Any(r => r.ContentImageId == i.Id)
                    && !db.GearImages.Any(g => g.ContentImageId == i.Id)
                    && !db.RecipeSteps.Any(s => s.ContentImageId == i.Id))
                .ToListAsync(ct);

            if (orphans.Count == 0) return;

            // Rows first: a failure here leaves the files in place, which the next sweep picks
            // up. Deleting the files first would leave rows pointing at nothing.
            var storedPaths = orphans
                .SelectMany(image => image.Variants.Select(variant => variant.StoredPath).Append(image.StoredPath))
                .ToList();

            db.ContentImages.RemoveRange(orphans);
            await db.SaveChangesAsync(ct);

            foreach (var path in storedPaths)
                images.Delete(path);
        }
    }
}
