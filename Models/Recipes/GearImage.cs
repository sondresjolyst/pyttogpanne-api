using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pyttogpanne_api.Models.Recipes
{
    /// <summary>A photo on a gear item or trail tip. The one sorted first is the cover.</summary>
    public class GearImage : IGalleryImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GearItemId { get; set; }

        [ForeignKey(nameof(GearItemId))]
        public GearItem? GearItem { get; set; }

        [Required]
        [MaxLength(32)]
        public string ContentImageId { get; set; } = string.Empty;

        [ForeignKey(nameof(ContentImageId))]
        public ContentImage? ContentImage { get; set; }

        public int SortOrder { get; set; }

        [MaxLength(200)]
        public string? Caption { get; set; }
    }
}
