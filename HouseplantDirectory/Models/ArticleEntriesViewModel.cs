using HouseplantDirectory.Data.Models;

namespace HouseplantDirectory.Models
{
    public class ArticleEntriesViewModel
    {
        public List<Article> Entries { get; set; } = new();
        public SearchingAndSorting SearchAndSort { get; set; }
        public Paging Paging { get; set; }
    }
}
