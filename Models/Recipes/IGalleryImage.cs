namespace pyttogpanne_api.Models.Recipes
{
    /// <summary>
    /// A photo in an ordered gallery. Recipes and gear items own separate tables, but the
    /// ordering rule is the same for both: the photo sorted first is the cover.
    /// </summary>
    public interface IGalleryImage
    {
        int Id { get; }
        string ContentImageId { get; set; }
        int SortOrder { get; set; }
        string? Caption { get; set; }
    }
}
