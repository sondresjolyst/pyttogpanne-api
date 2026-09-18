using System.ComponentModel.DataAnnotations;

namespace pyttogpanne_api.Models.Recipes
{
    public class RecipeIngredient
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        public int SortOrder { get; set; }

        /// <summary>Optional heading a run of ingredients belongs under, e.g. "Til dressingen".</summary>
        [MaxLength(80)]
        public string? GroupName { get; set; }

        [MaxLength(40)]
        public string? Amount { get; set; }

        [MaxLength(40)]
        public string? Unit { get; set; }

        [MaxLength(160)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Note { get; set; }
    }
}
