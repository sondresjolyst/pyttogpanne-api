using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Features.Recipes
{
    public static class RecipeMapping
    {
        public static RecipeSummaryDto ToSummary(Recipe r) => Fill(new RecipeSummaryDto(), r);

        public static RecipeDetailDto ToDetail(Recipe r)
        {
            var dto = Fill(new RecipeDetailDto(), r);
            dto.PrepMinutes = r.PrepMinutes;
            dto.CookMinutes = r.CookMinutes;
            dto.Tips = r.Tips;
            dto.PublishedAt = r.PublishedAt;
            dto.Ingredients = [.. r.Ingredients.OrderBy(i => i.SortOrder).Select(i => new RecipeIngredientDto
            {
                Id = i.Id,
                SortOrder = i.SortOrder,
                GroupName = i.GroupName,
                Amount = i.Amount,
                Unit = i.Unit,
                Name = i.Name,
                Note = i.Note
            })];
            dto.Images = GalleryImages.ToDtos(r.Images);
            dto.Steps = [.. r.Steps.OrderBy(s => s.SortOrder).Select(s => new RecipeStepDto
            {
                Id = s.Id,
                SortOrder = s.SortOrder,
                Text = s.Text,
                ContentImageId = s.ContentImageId
            })];
            return dto;
        }

        public static RecipeCategoryDto ToDto(RecipeCategory c) => new()
        {
            Id = c.Id,
            Key = c.Key,
            Name = c.Name,
            SortOrder = c.SortOrder
        };

        private static T Fill<T>(T dto, Recipe r) where T : RecipeSummaryDto
        {
            dto.Id = r.Id;
            dto.Slug = r.Slug;
            dto.Title = r.Title;
            dto.Intro = r.Intro;
            dto.Servings = r.Servings;
            dto.TotalMinutes = r.PrepMinutes.HasValue || r.CookMinutes.HasValue
                ? (r.PrepMinutes ?? 0) + (r.CookMinutes ?? 0)
                : null;
            dto.Difficulty = r.Difficulty.ToString();
            dto.CoverImageId = GalleryImages.Cover(r.Images);
            dto.IsPublished = r.IsPublished;
            dto.IsAdvertising = r.IsAdvertising;
            dto.Advertiser = r.Advertiser;
            dto.UpdatedAt = r.UpdatedAt;
            dto.Categories = [.. r.Categories
                .Where(l => l.RecipeCategory != null)
                .Select(l => ToDto(l.RecipeCategory!))
                .OrderBy(c => c.SortOrder)];
            return dto;
        }
    }
}
