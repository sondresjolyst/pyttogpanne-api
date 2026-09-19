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

        public int SortOrder { get; set; }

        public List<GearImage> Images { get; set; } = [];


        /// <summary>
        /// Set when the item is advertising: free products, a discount or a paid partnership.
        /// Forbrukertilsynet treats free products with an underlying expectation of exposure as
        /// advertising even without an agreement, so this is the author's call to make per item.
        /// </summary>
        public bool IsAdvertising { get; set; }

        /// <summary>Who the advertiser is, shown next to the label when it is filled in.</summary>
        [MaxLength(120)]
        public string? Advertiser { get; set; }

        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
