using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Helpers;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Infrastructure
{
    /// <summary>
    /// Invented recipes for a development database, so the app has something to show before
    /// Pyttogpanne has written anything. Only runs in Development, and only while the
    /// recipe table is empty.
    /// </summary>
    public static class SampleRecipes
    {
        private record Sample(
            string Title,
            string Intro,
            int Servings,
            int PrepMinutes,
            int CookMinutes,
            RecipeDifficulty Difficulty,
            string CategoryKey,
            string? Tips,
            (string? Group, string? Amount, string? Unit, string Name)[] Ingredients,
            string[] Steps);

        private static readonly Sample[] All =
        [
            new(
                "Fiskepytt med rotgrønnsaker",
                "Ett panne, ein brennar. Skjer opp heime, så er det berre å steike når du kjem fram.",
                2, 15, 20, RecipeDifficulty.Enkel, "middag",
                "Har du att kokt potet frå dagen før, går steiketida ned til det halve.",
                [
                    (null, "400", "g", "torsk i terningar"),
                    (null, "4", "stk", "kokte poteter"),
                    (null, "1", "stk", "gulrot"),
                    (null, "1", "stk", "lauk"),
                    (null, "2", "ss", "smør"),
                    ("Til slutt", "1", "neve", "gressløk"),
                    ("Til slutt", null, null, "salt og pepar")
                ],
                [
                    "Skjer potet, gulrot og lauk i terningar. Dette kan gjerast heime.",
                    "Smelt smøret i panna og steik lauk og gulrot til dei mjuknar.",
                    "Ha i poteta og steik til ho får farge.",
                    "Legg fisken øvst, legg på lok og la det stå i fem minutt.",
                    "Vend forsiktig saman, smak til med salt og pepar, og strø over gressløk."
                ]),
            new(
                "Turgrøt med bær",
                "Frukost som kokar medan du pakkar saman teltet.",
                2, 2, 8, RecipeDifficulty.Enkel, "frokost",
                null,
                [
                    (null, "2", "dl", "havregryn"),
                    (null, "5", "dl", "vatn"),
                    (null, "1", "klype", "salt"),
                    (null, "1", "neve", "blåbær"),
                    (null, "2", "ss", "brunt sukker")
                ],
                [
                    "Kok opp vatn og salt.",
                    "Rør inn havregryna og la det koke i fem minutt. Rør no og då så det ikkje brenn seg.",
                    "Ta panna av varmen og la grøten svelle eit par minutt.",
                    "Strø over bær og sukker."
                ]),
            new(
                "Pølsegryte i panna",
                "Mat som toler regn og ein brennar som blafrar.",
                4, 10, 15, RecipeDifficulty.Enkel, "middag",
                "Byt pølsene mot det du har att i kjøleskapet. Retten toler det meste.",
                [
                    (null, "6", "stk", "grillpølser"),
                    (null, "1", "boks", "hakka tomat"),
                    (null, "1", "boks", "kvite bønner"),
                    (null, "1", "stk", "paprika"),
                    (null, "1", "ts", "paprikapulver"),
                    (null, "1", "ss", "olje")
                ],
                [
                    "Skjer pølsene i skiver og paprikaen i strimlar.",
                    "Brun pølsene i olje i eit par minutt.",
                    "Ha i paprika og paprikapulver og steik eit minutt til.",
                    "Hell over tomat og bønner, og la det småkoke til det tjuknar.",
                    "Smak til med salt."
                ])
        ];

        public static async Task EnsureAsync(ApplicationDbContext db, CancellationToken ct = default)
        {
            if (await db.Recipes.AnyAsync(ct)) return;

            var categories = await db.RecipeCategories.ToDictionaryAsync(category => category.Key, ct);

            foreach (var sample in All)
            {
                var recipe = new Recipe
                {
                    Slug = Slugify.Create(sample.Title),
                    Title = sample.Title,
                    Intro = sample.Intro,
                    Servings = sample.Servings,
                    PrepMinutes = sample.PrepMinutes,
                    CookMinutes = sample.CookMinutes,
                    Difficulty = sample.Difficulty,
                    Tips = sample.Tips,
                    IsPublished = true,
                    PublishedAt = DateTime.UtcNow,
                };

                var order = 0;
                foreach (var (group, amount, unit, name) in sample.Ingredients)
                {
                    recipe.Ingredients.Add(new RecipeIngredient
                    {
                        SortOrder = order++,
                        GroupName = group,
                        Amount = amount,
                        Unit = unit,
                        Name = name,
                    });
                }

                order = 0;
                foreach (var text in sample.Steps)
                    recipe.Steps.Add(new RecipeStep { SortOrder = order++, Text = text });

                if (categories.TryGetValue(sample.CategoryKey, out var category))
                    recipe.Categories.Add(new RecipeCategoryLink { RecipeCategoryId = category.Id });

                db.Recipes.Add(recipe);
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
