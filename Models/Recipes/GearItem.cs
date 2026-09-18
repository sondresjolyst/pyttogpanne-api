using System.ComponentModel.DataAnnotations;

namespace pyttogpanne_api.Models.Recipes
{
    public enum GearItemKind
    {
        Utstyr,
        Tips
    }

    public class GearItem
    {
        public int Id { get; set; }

        [MaxLength(160)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(160)]
        public string Title { get; set; } = string.Empty;

        public GearItemKind Kind { get; set; } = GearItemKind.Utstyr;

        [MaxLength(500)]
        public string? Summary { get; set; }

        /// <summary>Markdown.</summary>
        public string Body { get; set; } = string.Empty;

        [MaxLength(32)]
        public string? ContentImageId { get; set; }
        public ContentImage? ContentImage { get; set; }

        public int SortOrder { get; set; }

        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
