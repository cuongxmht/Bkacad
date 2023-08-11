using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCapp.Models;

namespace MVCapp.Controllers
{
    public class SubNavController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubNavController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubNav
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SubNav.Include(s => s.NavBar);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SubNav/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.SubNav == null)
            {
                return NotFound();
            }

            var subNav = await _context.SubNav
                .Include(s => s.NavBar)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subNav == null)
            {
                return NotFound();
            }

            return View(subNav);
        }

        // GET: SubNav/Create
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.NavBar, "Id", "Id");
            return View();
        }

        // POST: SubNav/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Title")] SubNav subNav)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subNav);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.NavBar, "Id", "Id", subNav.ParentId);
            return View(subNav);
        }

        // GET: SubNav/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SubNav == null)
            {
                return NotFound();
            }

            var subNav = await _context.SubNav.FindAsync(id);
            if (subNav == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(_context.NavBar, "Id", "Id", subNav.ParentId);
            return View(subNav);
        }

        // POST: SubNav/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ParentId,Title")] SubNav subNav)
        {
            if (id != subNav.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subNav);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubNavExists(subNav.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.NavBar, "Id", "Id", subNav.ParentId);
            return View(subNav);
        }

        // GET: SubNav/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.SubNav == null)
            {
                return NotFound();
            }

            var subNav = await _context.SubNav
                .Include(s => s.NavBar)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subNav == null)
            {
                return NotFound();
            }

            return View(subNav);
        }

        // POST: SubNav/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SubNav == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SubNav'  is null.");
            }
            var subNav = await _context.SubNav.FindAsync(id);
            if (subNav != null)
            {
                _context.SubNav.Remove(subNav);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubNavExists(string id)
        {
          return (_context.SubNav?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
