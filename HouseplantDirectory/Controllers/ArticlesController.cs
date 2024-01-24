using HouseplantDirectory.Constants;
using HouseplantDirectory.Data;
using HouseplantDirectory.Data.Models;
using HouseplantDirectory.Enums;
using HouseplantDirectory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HouseplantDirectory.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public ArticlesController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: Articles
        public IActionResult Index([FromQuery] string search = "", [FromQuery] SortOption sort = SortOption.AlphabeticalAsc,
            [FromQuery] int offset = 0, [FromQuery] int limit = 20)
        {
            var queryable = _context.Articles.Include(a => a.ArticleType).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                queryable = queryable.Where(s => s.Name.Contains(search) || s.ArticleType.Name.Contains(search));
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
                case SortOption.ActiveAsc:
                    queryable = queryable.OrderBy(a => a.IsActive);
                    break;
                case SortOption.ActiveDesc:
                    queryable = queryable.OrderByDescending(a => a.IsActive);
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
                    Source = SearchingAndSorting.SourceOption.AdminArticles
                },
                Paging = new Paging
                (
                    queryable.Count(),
                    offset,
                    limit,
                    (oft, lmt) => $"/Articles?sort={sort}&search={HttpUtility.UrlEncode(search)}&offset={offset}&limit={limit}"
                )
            };
            return View(result);
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["ArticleTypeId"] = new SelectList(_context.ArticleTypes.Where(s => s.IsActive), "Id", "Name");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Writer,Description,Image,ArticleTypeId,IsActive")] ArticleCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var article = new Article
                {
                    Name = model.Name,
                    Writer = model.Writer,
                    Description = model.Description,
                    ArticleTypeId = model.ArticleTypeId,
                    IsActive = model.IsActive,
                    Image = "",
                    DateCreated = DateTimeOffset.Now,
                    DateUpdated = DateTimeOffset.Now
                };
                _context.Add(article);
                await _context.SaveChangesAsync();

                if (model.Image != null)
                {
                    article.Image = $"{article.Id}{System.IO.Path.GetExtension(model.Image.FileName)}";
                    var path = System.IO.Path.Combine(_config.GetValue<string>("ImagesPath"), AppConstants.ImagesFolder, article.Image);
                    using var stream = new System.IO.FileStream(path, FileMode.OpenOrCreate);
                    model.Image.CopyTo(stream);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleTypeId"] = new SelectList(_context.ArticleTypes.Where(s => s.IsActive), "Id", "Name", model.ArticleTypeId);
            return View(model);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["ArticleTypeId"] = new SelectList(_context.ArticleTypes.Where(s => s.IsActive), "Id", "Name", article.ArticleTypeId);
            return View(new ArticleUpdateModel
            {
                Id = article.Id,
                Name = article.Name,
                Writer = article.Writer,
                Description = article.Description,
                ArticleTypeId = article.ArticleTypeId,
                ImageName = article.Image,
                IsActive = article.IsActive
            });
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Writer,Description,Image,ArticleTypeId,IsActive,RemoveImage")] ArticleUpdateModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingEtnry = await _context.Articles.FindAsync(id);
                if (existingEtnry == null)
                {
                    return NotFound();
                }
                existingEtnry.Name = model.Name;
                existingEtnry.Writer = model.Writer;
                existingEtnry.Description = model.Description;
                existingEtnry.ArticleTypeId = model.ArticleTypeId;
                existingEtnry.IsActive = model.IsActive;
                existingEtnry.DateUpdated = DateTimeOffset.Now;

                if (model.RemoveImage)
                {
                    existingEtnry.Image = "";
                }
                else if (model.Image != null)
                {
                    existingEtnry.Image = $"{existingEtnry.Id}{System.IO.Path.GetExtension(model.Image.FileName)}";
                    var path = System.IO.Path.Combine(_config.GetValue<string>("ImagesPath"), AppConstants.ImagesFolder, existingEtnry.Image);
                    using var stream = new System.IO.FileStream(path, FileMode.OpenOrCreate);
                    model.Image.CopyTo(stream);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleTypeId"] = new SelectList(_context.ArticleTypes.Where(s => s.IsActive), "Id", "Name", model.ArticleTypeId);
            return View(model);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }

    public class ArticleCreateModel
    {
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Writer { get; set; }
        [MaxLength(4000)]
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public int ArticleTypeId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ArticleUpdateModel : ArticleCreateModel
    {
        public int Id { get; set; }
        public bool RemoveImage { get; set; }
        public string? ImageName { get; set; }
    }
}
