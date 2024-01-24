using HouseplantDirectory.Data;
using HouseplantDirectory.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseplantDirectory.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CalculatorValuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalculatorValuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CalculatorValues
        public async Task<IActionResult> Index()
        {
            return View(await _context.CalculatorValues.ToListAsync());
        }

        // GET: CalculatorValues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calculatorValue = await _context.CalculatorValues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (calculatorValue == null)
            {
                return NotFound();
            }

            return View(calculatorValue);
        }

        // GET: CalculatorValues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CalculatorValues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SubstanceType,QuantityType,LiquidAmount,SubstanceAmount,IsActive")] CalculatorValue calculatorValue)
        {
            if (ModelState.IsValid)
            {
                calculatorValue.DateCreated = calculatorValue.DateUpdated = DateTimeOffset.Now;
                _context.Add(calculatorValue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(calculatorValue);
        }

        // GET: CalculatorValues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calculatorValue = await _context.CalculatorValues.FindAsync(id);
            if (calculatorValue == null)
            {
                return NotFound();
            }
            return View(calculatorValue);
        }

        // POST: CalculatorValues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SubstanceType,QuantityType,LiquidAmount,SubstanceAmount,IsActive")] CalculatorValue calculatorValue)
        {
            if (id != calculatorValue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingEtnry = await _context.CalculatorValues.FindAsync(id);
                if (existingEtnry == null)
                {
                    return NotFound();
                }
                existingEtnry.Name = calculatorValue.Name;
                existingEtnry.SubstanceAmount = calculatorValue.SubstanceAmount;
                existingEtnry.LiquidAmount = calculatorValue.LiquidAmount;
                existingEtnry.QuantityType = calculatorValue.QuantityType;
                existingEtnry.SubstanceType = calculatorValue.SubstanceType;
                existingEtnry.IsActive = calculatorValue.IsActive;
                existingEtnry.DateUpdated = DateTimeOffset.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(calculatorValue);
        }

        // GET: CalculatorValues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calculatorValue = await _context.CalculatorValues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (calculatorValue == null)
            {
                return NotFound();
            }

            return View(calculatorValue);
        }

        // POST: CalculatorValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calculatorValue = await _context.CalculatorValues.FindAsync(id);
            if (calculatorValue != null)
            {
                _context.CalculatorValues.Remove(calculatorValue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CalculatorValueExists(int id)
        {
            return _context.CalculatorValues.Any(e => e.Id == id);
        }
    }
}
