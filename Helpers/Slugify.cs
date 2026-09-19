using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace pyttogpanne_api.Helpers
{
    public static partial class Slugify
    {
        public static string Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var normalized = input
                .Replace("æ", "ae").Replace("Æ", "ae")
                .Replace("ø", "o").Replace("Ø", "o")
                .Replace("å", "a").Replace("Å", "a");

            normalized = normalized.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            var slug = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
            slug = NonSlugChars().Replace(slug, "-");
            slug = MultiDash().Replace(slug, "-").Trim('-');
            return slug;
        }


        /// <summary>
        /// A slug for <paramref name="title"/> that none of <paramref name="taken"/> already holds,
        /// adding -2, -3 and so on until it is free. Callers pass the slugs of every other row,
        /// leaving out the row being edited so it does not collide with itself.
        /// </summary>
        public static async Task<string> UniqueAsync(IQueryable<string> taken, string title, string fallback, CancellationToken ct = default)
        {
            var baseSlug = Create(title);
            if (string.IsNullOrEmpty(baseSlug)) baseSlug = fallback;

            var used = await taken.ToListAsync(ct);

            var slug = baseSlug;
            var suffix = 2;
            while (used.Contains(slug))
            {
                slug = $"{baseSlug}-{suffix}";
                suffix++;
            }
            return slug;
        }

        [GeneratedRegex("[^a-z0-9]+")]
        private static partial Regex NonSlugChars();

        [GeneratedRegex("-{2,}")]
        private static partial Regex MultiDash();
    }
}
