using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace HouseplantDirectory.Data.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Article
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Display(Name = "Название статьи")]
        public string Name { get; set; }
        [MaxLength(250)]
        [Display(Name = "Автор статьи")]
        public string Writer { get; set; }
        [MaxLength(4000)]
        [Display(Name = "Статья")]
        public string Description { get; set; }
        [MaxLength(250)]
        [Display(Name = "Изображение")]
        public string Image { get; set; }
        public int ArticleTypeId { get; set; }
        public ArticleType ArticleType { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; }

        public List<FavoriteArticle> FavoriteArticles { get; set; } = new();
    }
}
