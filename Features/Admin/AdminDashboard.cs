using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Admin;
using SjolystInnovation.AppKit.Storage;
using SjolystInnovation.AppKit.Storage.Images;

namespace pyttogpanne_api.Features.Admin
{
    /// <summary>Admin dashboard: aggregate stats and the sampler behind the daily history.</summary>
    public static class AdminDashboard
    {
        public static async Task<IResult> GetStats(ApplicationDbContext db, IAppKitStorageUsage storage, CancellationToken ct)
        {
            var usage = await storage.GetAsync([AppKitContentImages.StoreName], ct);

            return TypedResults.Ok(new AdminStatsDto
            {
                TotalUsers = await db.Users.CountAsync(u => !u.IsDeleted, ct),
                PublishedRecipes = await db.Recipes.CountAsync(r => r.IsPublished, ct),
                DraftRecipes = await db.Recipes.CountAsync(r => !r.IsPublished, ct),
                RecipeCategories = await db.RecipeCategories.CountAsync(ct),
                GearItemCount = await db.GearItems.CountAsync(ct),
                ContentImages = await db.ContentImages.CountAsync(ct),
                StorageUsedBytes = usage.StorageUsedBytes,
                DiskTotalBytes = usage.DiskTotalBytes,
                DiskFreeBytes = usage.DiskFreeBytes
            });
        }

        /// <summary>The live totals behind the daily history. AppKit.AdminStats stores one row per completed day.</summary>
        public static async Task<DailyStatSnapshot> SampleAsync(ApplicationDbContext db, CancellationToken ct) => new()
        {
            TotalUsers = await db.Users.CountAsync(u => !u.IsDeleted, ct),
            PublishedRecipes = await db.Recipes.CountAsync(r => r.IsPublished, ct),
            DraftRecipes = await db.Recipes.CountAsync(r => !r.IsPublished, ct),
            GearItemCount = await db.GearItems.CountAsync(ct),
            ContentImages = await db.ContentImages.CountAsync(ct)
        };

        public class Endpoints : IEndpoint
        {
            public void Map(IEndpointRouteBuilder app)
            {
                var group = app.MapGroup("/api/admin").RequireAuthorization(Policies.Admin);
                group.MapGet("stats", GetStats);
            }
        }
    }
}
