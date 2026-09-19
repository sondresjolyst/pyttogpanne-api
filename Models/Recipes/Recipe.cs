using System.ComponentModel.DataAnnotations;

namespace pyttogpanne_api.Models.Recipes
{
    public enum RecipeDifficulty
    {
        Enkel,
        Middels,
        Avansert
    }

    public class Recipe
    {
        public int Id { get; set; }

        [MaxLength(160)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(160)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Intro { get; set; }

        public int Servings { get; set; } = 2;

        public int? PrepMinutes { get; set; }

        public int? CookMinutes { get; set; }

        public RecipeDifficulty Difficulty { get; set; } = RecipeDifficulty.Enkel;

        public string? Tips { get; set; }


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
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<RecipeImage> Images { get; set; } = [];
        public List<RecipeIngredient> Ingredients { get; set; } = [];
        public List<RecipeStep> Steps { get; set; } = [];
        public List<RecipeCategoryLink> Categories { get; set; } = [];
    }
}
