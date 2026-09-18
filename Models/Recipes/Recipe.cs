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

        [MaxLength(32)]
        public string? CoverImageId { get; set; }
        public ContentImage? CoverImage { get; set; }

        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<RecipeIngredient> Ingredients { get; set; } = [];
        public List<RecipeStep> Steps { get; set; } = [];
        public List<RecipeCategoryLink> Categories { get; set; } = [];
    }
}
