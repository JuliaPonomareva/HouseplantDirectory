using HouseplantDirectory.Enums;

namespace HouseplantDirectory.Models
{
    public class SearchingAndSorting
    {
        public string Searching { get; set; }
        public SortOption Sorting { get; set; }
        public SourceOption Source { get; set; }

        public enum SourceOption
        {
            AdminArticles = 0,
            Home = 1
        }
    }
}
