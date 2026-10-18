namespace pyttogpanne_api.Features.Admin
{
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int PublishedRecipes { get; set; }
        public int DraftRecipes { get; set; }
        public int RecipeCategories { get; set; }
        public int GearItemCount { get; set; }
        public int ContentImages { get; set; }
        public long StorageUsedBytes { get; set; }
        public long DiskTotalBytes { get; set; }
        public long DiskFreeBytes { get; set; }
    }
}
