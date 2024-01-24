using HouseplantDirectory.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HouseplantDirectory.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ArticleType> ArticleTypes { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<FavoriteArticle> FavoriteArticles { get; set; }
        public DbSet<CalculatorValue> CalculatorValues { get; set; }
    }

    public class ApplicationUser : IdentityUser<int>
    {
        public List<FavoriteArticle> FavoriteArticles { get; set; } = new();
    }
}
