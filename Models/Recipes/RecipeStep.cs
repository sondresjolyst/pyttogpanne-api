using System.ComponentModel.DataAnnotations;

namespace pyttogpanne_api.Models.Recipes
{
    public class RecipeStep
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        public int SortOrder { get; set; }

        public string Text { get; set; } = string.Empty;

        [MaxLength(32)]
        public string? ContentImageId { get; set; }
        public ContentImage? ContentImage { get; set; }
    }
}
