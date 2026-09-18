using System.ComponentModel.DataAnnotations;

namespace pyttogpanne_api.Models.Recipes
{
    /// <summary>
    /// A slug an offline client should drop: the recipe was deleted or unpublished. The row is
    /// removed again if a recipe reclaims the slug, so a re-published recipe syncs back in.
    /// </summary>
    public class DeletedRecipe
    {
        public int Id { get; set; }

        [MaxLength(160)]
        public string Slug { get; set; } = string.Empty;

        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }
}
