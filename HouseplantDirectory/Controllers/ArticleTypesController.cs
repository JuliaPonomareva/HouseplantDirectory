using HouseplantDirectory.Data;
using HouseplantDirectory.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseplantDirectory.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticleTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticleTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ArticleTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ArticleTypes.ToListAsync());
        }

        // GET: ArticleTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleType = await _context.ArticleTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articleType == null)
            {
                return NotFound();
            }

            return View(articleType);
        }

        // GET: ArticleTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ArticleTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsActive")] ArticleType articleType)
        {
            if (ModelState.IsValid)
            {
                articleType.DateCreated = articleType.DateUpdated = DateTimeOffset.Now;
                _context.Add(articleType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(articleType);
        }

        // GET: ArticleTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleType = await _context.ArticleTypes.FindAsync(id);
            if (articleType == null)
            {
                return NotFound();
            }
            return View(articleType);
        }

        // POST: ArticleTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsActive")] ArticleType articleType)
        {
            if (id != articleType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingEtnry = await _context.ArticleTypes.FindAsync(id);
                if (existingEtnry == null)
                {
                    return NotFound();
                }
                existingEtnry.Name = articleType.Name;
                existingEtnry.IsActive = articleType.IsActive;
                existingEtnry.DateUpdated = DateTimeOffset.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(articleType);
        }

        // GET: ArticleTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleType = await _context.ArticleTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articleType == null)
            {
                return NotFound();
            }

            return View(articleType);
        }

        // POST: ArticleTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articleType = await _context.ArticleTypes.FindAsync(id);
            if (articleType != null)
            {
                _context.ArticleTypes.Remove(articleType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleTypeExists(int id)
        {
            return _context.ArticleTypes.Any(e => e.Id == id);
        }
    }
}
