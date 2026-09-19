using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Models.Admin;
using pyttogpanne_api.Models.Auth;
using pyttogpanne_api.Models.Content;
using pyttogpanne_api.Models.Recipes;

namespace pyttogpanne_api.Models
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<User, IdentityRole, string>(options)
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<LegalPage> LegalPages { get; set; }
        public DbSet<ContentImage> ContentImages { get; set; }
        public DbSet<ContentImageVariant> ContentImageVariants { get; set; }
        public DbSet<DailyStatSnapshot> DailyStatSnapshots { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeImage> RecipeImages { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<RecipeCategory> RecipeCategories { get; set; }
        public DbSet<RecipeCategoryLink> RecipeCategoryLinks { get; set; }
        public DbSet<GearItem> GearItems { get; set; }
        public DbSet<GearImage> GearImages { get; set; }
        public DbSet<DeletedRecipe> DeletedRecipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DailyStatSnapshot>()
                .HasIndex(s => s.Date)
                .IsUnique();

            modelBuilder.Entity<LegalPage>()
                .HasIndex(p => new { p.Key, p.Locale })
                .IsUnique();

            modelBuilder.Entity<ContentImageVariant>()
                .HasOne(v => v.ContentImage)
                .WithMany(i => i.Variants)
                .HasForeignKey(v => v.ContentImageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                .HasIndex(r => r.Slug)
                .IsUnique();

            modelBuilder.Entity<Recipe>()
                .Property(r => r.Difficulty)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<RecipeImage>()
                .HasIndex(i => new { i.RecipeId, i.ContentImageId })
                .IsUnique();

            modelBuilder.Entity<RecipeImage>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Images)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeImage>()
                .HasOne(i => i.ContentImage)
                .WithMany()
                .HasForeignKey(i => i.ContentImageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeStep>()
                .HasOne(s => s.Recipe)
                .WithMany(r => r.Steps)
                .HasForeignKey(s => s.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeStep>()
                .HasOne(s => s.ContentImage)
                .WithMany()
                .HasForeignKey(s => s.ContentImageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RecipeCategory>()
                .HasIndex(c => c.Key)
                .IsUnique();

            modelBuilder.Entity<RecipeCategoryLink>()
                .HasKey(l => new { l.RecipeId, l.RecipeCategoryId });

            modelBuilder.Entity<RecipeCategoryLink>()
                .HasOne(l => l.Recipe)
                .WithMany(r => r.Categories)
                .HasForeignKey(l => l.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeCategoryLink>()
                .HasOne(l => l.RecipeCategory)
                .WithMany(c => c.Recipes)
                .HasForeignKey(l => l.RecipeCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeletedRecipe>()
                .HasIndex(d => d.Slug)
                .IsUnique();

            modelBuilder.Entity<GearItem>()
                .HasIndex(g => g.Slug)
                .IsUnique();

            modelBuilder.Entity<GearItem>()
                .Property(g => g.Kind)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<GearImage>()
                .HasIndex(i => new { i.GearItemId, i.ContentImageId })
                .IsUnique();

            modelBuilder.Entity<GearImage>()
                .HasOne(i => i.GearItem)
                .WithMany(g => g.Images)
                .HasForeignKey(i => i.GearItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GearImage>()
                .HasOne(i => i.ContentImage)
                .WithMany()
                .HasForeignKey(i => i.ContentImageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
