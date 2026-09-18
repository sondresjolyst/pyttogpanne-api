using FluentValidation;

namespace pyttogpanne_api.Features.Recipes
{
    public class RecipeIngredientInput
    {
        public string? GroupName { get; set; }
        public string? Amount { get; set; }
        public string? Unit { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class RecipeStepInput
    {
        public string Text { get; set; } = string.Empty;
        public string? ContentImageId { get; set; }
    }

    public class RecipeInput
    {
        public string Title { get; set; } = string.Empty;
        public string? Intro { get; set; }
        public int Servings { get; set; } = 2;
        public int? PrepMinutes { get; set; }
        public int? CookMinutes { get; set; }
        public string Difficulty { get; set; } = "Enkel";
        public string? Tips { get; set; }
        public string? CoverImageId { get; set; }
        public bool IsPublished { get; set; }
        public List<int> CategoryIds { get; set; } = [];
        public List<RecipeIngredientInput> Ingredients { get; set; } = [];
        public List<RecipeStepInput> Steps { get; set; } = [];
    }

    public class RecipeValidator : AbstractValidator<RecipeInput>
    {
        public RecipeValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(160);
            RuleFor(x => x.Intro).MaximumLength(500);
            RuleFor(x => x.Servings).InclusiveBetween(1, 50);
            RuleFor(x => x.PrepMinutes).InclusiveBetween(0, 1440).When(x => x.PrepMinutes.HasValue);
            RuleFor(x => x.CookMinutes).InclusiveBetween(0, 1440).When(x => x.CookMinutes.HasValue);
            RuleFor(x => x.Difficulty)
                .Must(d => Enum.TryParse<Models.Recipes.RecipeDifficulty>(d, ignoreCase: true, out _))
                .WithMessage("Unknown difficulty.");
            RuleFor(x => x.Ingredients).NotEmpty().WithMessage("A recipe needs at least one ingredient.");
            RuleForEach(x => x.Ingredients).ChildRules(i =>
            {
                i.RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
                i.RuleFor(x => x.GroupName).MaximumLength(80);
                i.RuleFor(x => x.Amount).MaximumLength(40);
                i.RuleFor(x => x.Unit).MaximumLength(40);
                i.RuleFor(x => x.Note).MaximumLength(200);
            });
            RuleFor(x => x.Steps).NotEmpty().WithMessage("A recipe needs at least one step.");
            RuleForEach(x => x.Steps).ChildRules(s =>
            {
                s.RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
                s.RuleFor(x => x.ContentImageId).MaximumLength(32);
            });
        }
    }

    public class RecipeIngredientDto
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
        public string? GroupName { get; set; }
        public string? Amount { get; set; }
        public string? Unit { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class RecipeStepDto
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ContentImageId { get; set; }
    }

    public class RecipeCategoryDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class RecipeSummaryDto
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Intro { get; set; }
        public int Servings { get; set; }
        public int? TotalMinutes { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string? CoverImageId { get; set; }
        public bool IsPublished { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<RecipeCategoryDto> Categories { get; set; } = [];
    }

    public class RecipeDetailDto : RecipeSummaryDto
    {
        public int? PrepMinutes { get; set; }
        public int? CookMinutes { get; set; }
        public string? Tips { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<RecipeIngredientDto> Ingredients { get; set; } = [];
        public List<RecipeStepDto> Steps { get; set; } = [];
    }
}
