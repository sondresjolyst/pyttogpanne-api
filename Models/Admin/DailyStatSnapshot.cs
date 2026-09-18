using System.ComponentModel.DataAnnotations;

namespace pyttogpanne_api.Models.Admin
{
    public class DailyStatSnapshot
    {
        [Key]
        public int Id { get; set; }

        public DateOnly Date { get; set; }

        public int TotalUsers { get; set; }
        public int PublishedRecipes { get; set; }
        public int DraftRecipes { get; set; }
        public int GearItemCount { get; set; }
        public int ContentImages { get; set; }
    }
}
