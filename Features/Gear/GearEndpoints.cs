using FluentValidation;
using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Constants;
using pyttogpanne_api.Helpers;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Features.Gear
{
    /// <summary>Gear and trail tips: public read, admin write.</summary>
    public static class GearEndpoints
    {
        private static bool IsAdmin(HttpContext http) => http.User.IsInRole(RoleNames.Admin);

        public static async Task<IResult> GetAll(HttpContext http, ApplicationDbContext db, CancellationToken ct,
            string? kind = null, bool all = false)
        {
            var query = db.GearItems.AsNoTracking().AsQueryable();

            if (!(all && IsAdmin(http)))
                query = query.Where(g => g.IsPublished);

            if (!string.IsNullOrWhiteSpace(kind))
            {
                if (!Enum.TryParse<GearItemKind>(kind, ignoreCase: true, out var parsed))
                    return TypedResults.Problem("Unknown kind filter.", statusCode: StatusCodes.Status400BadRequest);
                query = query.Where(g => g.Kind == parsed);
            }

            var items = await query
                .OrderBy(g => g.SortOrder)
                .ThenBy(g => g.Title)
                .ToListAsync(ct);

            return TypedResults.Ok(items.Select(ToDto));
        }

        public static async Task<IResult> GetBySlug(string slug, HttpContext http, ApplicationDbContext db, CancellationToken ct)
        {
            var item = await db.GearItems.AsNoTracking().FirstOrDefaultAsync(g => g.Slug == slug, ct);
            if (item == null || (!item.IsPublished && !IsAdmin(http)))
                return TypedResults.NotFound();

            return TypedResults.Ok(ToDto(item));
        }

        public static async Task<IResult> Create(GearItemInput body, ApplicationDbContext db, CancellationToken ct)
        {
            var item = new GearItem
            {
                Slug = await UniqueSlugAsync(body.Title, null, db, ct),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            Apply(item, body);

            db.GearItems.Add(item);
            await db.SaveChangesAsync(ct);
            return TypedResults.Created($"/api/gear/{item.Slug}", ToDto(item));
        }

        public static async Task<IResult> Update(int id, GearItemInput body, ApplicationDbContext db, CancellationToken ct)
        {
            var item = await db.GearItems.FirstOrDefaultAsync(g => g.Id == id, ct);
            if (item == null) return TypedResults.NotFound();

            if (!string.Equals(item.Title, body.Title.Trim(), StringComparison.Ordinal))
                item.Slug = await UniqueSlugAsync(body.Title, item.Id, db, ct);

            Apply(item, body);
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);
            return TypedResults.Ok(ToDto(item));
        }

        public static async Task<IResult> Delete(int id, ApplicationDbContext db, CancellationToken ct)
        {
            var item = await db.GearItems.FirstOrDefaultAsync(g => g.Id == id, ct);
            if (item == null) return TypedResults.NotFound();

            db.GearItems.Remove(item);
            await db.SaveChangesAsync(ct);
            return TypedResults.NoContent();
        }

        private static void Apply(GearItem item, GearItemInput body)
        {
            item.Title = body.Title.Trim();
            item.Kind = Enum.Parse<GearItemKind>(body.Kind, ignoreCase: true);
            item.Summary = string.IsNullOrWhiteSpace(body.Summary) ? null : body.Summary.Trim();
            item.Body = body.Body.Trim();
            item.ContentImageId = string.IsNullOrWhiteSpace(body.ContentImageId) ? null : body.ContentImageId.Trim();
            item.SortOrder = body.SortOrder;
            item.IsPublished = body.IsPublished;
        }

        private static GearItemDto ToDto(GearItem g) => new()
        {
            Id = g.Id,
            Slug = g.Slug,
            Title = g.Title,
            Kind = g.Kind.ToString(),
            Summary = g.Summary,
            Body = g.Body,
            ContentImageId = g.ContentImageId,
            SortOrder = g.SortOrder,
            IsPublished = g.IsPublished,
            UpdatedAt = g.UpdatedAt
        };

        private static async Task<string> UniqueSlugAsync(string title, int? excludeId, ApplicationDbContext db, CancellationToken ct)
        {
            var baseSlug = Slugify.Create(title);
            if (string.IsNullOrEmpty(baseSlug)) baseSlug = "utstyr";

            var slug = baseSlug;
            var suffix = 2;
            while (await db.GearItems.AnyAsync(g => g.Slug == slug && g.Id != excludeId, ct))
            {
                slug = $"{baseSlug}-{suffix}";
                suffix++;
            }
            return slug;
        }

        public class Endpoints : IEndpoint
        {
            public void Map(IEndpointRouteBuilder app)
            {
                var group = app.MapGroup("/api/gear");
                group.MapGet("", GetAll).AllowAnonymous();
                group.MapGet("{slug}", GetBySlug).AllowAnonymous();
                group.MapPost("", Create).RequireAuthorization(Policies.Admin).WithValidation<GearItemInput>();
                group.MapPut("{id:int}", Update).RequireAuthorization(Policies.Admin).WithValidation<GearItemInput>();
                group.MapDelete("{id:int}", Delete).RequireAuthorization(Policies.Admin);
            }
        }
    }

    public class GearItemInput
    {
        public string Title { get; set; } = string.Empty;
        public string Kind { get; set; } = "Utstyr";
        public string? Summary { get; set; }
        public string Body { get; set; } = string.Empty;
        public string? ContentImageId { get; set; }
        public int SortOrder { get; set; }
        public bool IsPublished { get; set; }
    }

    public class GearItemValidator : AbstractValidator<GearItemInput>
    {
        public GearItemValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(160);
            RuleFor(x => x.Kind)
                .Must(k => Enum.TryParse<GearItemKind>(k, ignoreCase: true, out _))
                .WithMessage("Unknown kind.");
            RuleFor(x => x.Summary).MaximumLength(500);
            RuleFor(x => x.Body).NotEmpty();
            RuleFor(x => x.ContentImageId).MaximumLength(32);
        }
    }

    public class GearItemDto
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Body { get; set; } = string.Empty;
        public string? ContentImageId { get; set; }
        public int SortOrder { get; set; }
        public bool IsPublished { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
