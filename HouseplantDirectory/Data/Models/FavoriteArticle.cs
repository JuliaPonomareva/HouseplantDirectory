using Microsoft.EntityFrameworkCore;

namespace HouseplantDirectory.Data.Models
{
    [Index(nameof(ArticleId), nameof(ApplicationUserId), IsUnique = true)]
    public class FavoriteArticle
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}
