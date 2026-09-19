using FluentValidation;
using pyttogpanne_api.Helpers;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Infrastructure
{
    /// <summary>A photo as the editor sends it. The order of the list is the order shown.</summary>
    public class GalleryImageInput
    {
        public string ContentImageId { get; set; } = string.Empty;
        public string? Caption { get; set; }
    }

    public class GalleryImageDto
    {
        public int Id { get; set; }
        public string ContentImageId { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public string? Caption { get; set; }
    }

    public class GalleryImageValidator : AbstractValidator<GalleryImageInput>
    {
        public GalleryImageValidator()
        {
            RuleFor(x => x.ContentImageId).NotEmpty().MaximumLength(32);
            RuleFor(x => x.Caption).MaximumLength(200);
        }
    }

    /// <summary>
    /// The gallery rules recipes and gear items share: the same photo is only listed once, the
    /// list order is the sort order, and the first photo is the cover.
    /// </summary>
    public static class GalleryImages
    {
        public static void Replace<T>(ICollection<T> images, IEnumerable<GalleryImageInput> input)
            where T : class, IGalleryImage, new()
        {
            images.Clear();

            var order = 0;
            foreach (var image in input.DistinctBy(i => i.ContentImageId))
            {
                images.Add(new T
                {
                    ContentImageId = image.ContentImageId.Trim(),
                    Caption = Text.Trimmed(image.Caption),
                    SortOrder = order++,
                });
            }
        }

        public static string? Cover(IEnumerable<IGalleryImage> images) =>
            images.OrderBy(i => i.SortOrder).FirstOrDefault()?.ContentImageId;

        public static List<GalleryImageDto> ToDtos<T>(IEnumerable<T> images)
            where T : IGalleryImage =>
            [.. images.OrderBy(i => i.SortOrder).Select(i => new GalleryImageDto
            {
                Id = i.Id,
                ContentImageId = i.ContentImageId,
                SortOrder = i.SortOrder,
                Caption = i.Caption,
            })];
    }
}
