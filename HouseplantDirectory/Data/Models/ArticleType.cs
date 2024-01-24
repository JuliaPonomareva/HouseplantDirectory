using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseplantDirectory.Data.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class ArticleType
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Display(Name = "Введите тип статьи")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
    }
}
