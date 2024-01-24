using HouseplantDirectory.Data;
using HouseplantDirectory.Enums;
using HouseplantDirectory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Web;

namespace HouseplantDirectory.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index([FromQuery] string search = "", [FromQuery] SortOption sort = SortOption.AlphabeticalAsc,
            [FromQuery] int offset = 0, [FromQuery] int limit = 20)
        {
            var queryable = _context.Articles.Include(a => a.ArticleType).Where(s => s.IsActive).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                queryable = queryable.Where(s => s.Name.Contains(search) || s.Description.Contains(search) || s.ArticleType.Name.Contains(search));
            }
            switch (sort)
            {
                case SortOption.AlphabeticalAsc:
                    queryable = queryable.OrderBy(a => a.Name);
                    break;
                case SortOption.AlphabeticalDesc:
                    queryable = queryable.OrderByDescending(a => a.Name);
                    break;
                case SortOption.CreatedAsc:
                    queryable = queryable.OrderBy(a => a.Id);
                    break;
                case SortOption.CreatedDesc:
                    queryable = queryable.OrderByDescending(a => a.Id);
                    break;
                default:
                    break;
            }

            var total = queryable.Count();
            var entries = queryable.Skip(offset).Take(limit).ToList();

            var result = new ArticleEntriesViewModel
            {
                Entries = entries,
                SearchAndSort = new SearchingAndSorting
                {
                    Searching = search,
                    Sorting = sort,
                    Source = SearchingAndSorting.SourceOption.Home
                },
                Paging = new Paging
                (
                    queryable.Count(),
                    offset,
                    limit,
                    (oft, lmt) => $"/?sort={sort}&search={HttpUtility.UrlEncode(search)}&offset={offset}&limit={limit}"
                )
            };
            return View(result);
        }

        [HttpGet("/Home/Articles/{id}")]
        public IActionResult Article([FromRoute] int id)
        {
            var article = _context.Articles.Include(s => s.ArticleType).FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        public IActionResult Calculator()
        {
            var items = _context.CalculatorValues.Where(s => s.IsActive).ToList();
            return View(items);
        }

        [Authorize]
        public IActionResult Favorites()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorites = _context.FavoriteArticles.Include(s => s.Article).ThenInclude(s => s.ArticleType)
                .Where(s => s.ApplicationUserId == userId && s.Article.IsActive)
                .OrderByDescending(s => s.Id)
                .Select(s => s.Article)
                .ToList();
            return View(favorites);
        }

        [Authorize]
        [HttpPost("/Home/Favorites/{articleId}/Add")]
        public IActionResult FavoritesAdd([FromRoute] int articleId)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorite = _context.FavoriteArticles.FirstOrDefault(a => a.ArticleId == articleId && a.ApplicationUserId == userId);
            if (favorite == null)
            {
                _context.FavoriteArticles.Add(new Data.Models.FavoriteArticle
                {
                    ArticleId = articleId,
                    ApplicationUserId = userId,
                    DateCreated = DateTimeOffset.Now
                });
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Favorites));
        }

        [Authorize]
        [HttpPost("/Home/Favorites/{articleId}/Remove")]
        public IActionResult FavoritesRemove([FromRoute] int articleId)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorite = _context.FavoriteArticles.FirstOrDefault(a => a.ArticleId == articleId && a.ApplicationUserId == userId);
            if (favorite != null)
            {
                _context.FavoriteArticles.Remove(favorite);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Favorites));
        }

        public IActionResult FAQ()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
