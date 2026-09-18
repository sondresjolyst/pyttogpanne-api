using System.ComponentModel.DataAnnotations;

namespace pyttogpanne_api.Models.Recipes
{
    public class RecipeCategory
    {
        public int Id { get; set; }

        [MaxLength(60)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public List<RecipeCategoryLink> Recipes { get; set; } = [];
    }

    public class RecipeCategoryLink
    {
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        public int RecipeCategoryId { get; set; }
        public RecipeCategory? RecipeCategory { get; set; }
    }
}
