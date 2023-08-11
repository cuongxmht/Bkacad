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
    public class NavBarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NavBarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NavBar
        public async Task<IActionResult> Index()
        {
              return _context.NavBar != null ? 
                          View(await _context.NavBar.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.NavBar'  is null.");
        }

        // GET: NavBar/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.NavBar == null)
            {
                return NotFound();
            }

            var navBar = await _context.NavBar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (navBar == null)
            {
                return NotFound();
            }

            return View(navBar);
        }

        // GET: NavBar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NavBar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] NavBar navBar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(navBar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(navBar);
        }

        // GET: NavBar/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.NavBar == null)
            {
                return NotFound();
            }

            var navBar = await _context.NavBar.FindAsync(id);
            if (navBar == null)
            {
                return NotFound();
            }
            return View(navBar);
        }

        // POST: NavBar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title")] NavBar navBar)
        {
            if (id != navBar.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(navBar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NavBarExists(navBar.Id))
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
            return View(navBar);
        }

        // GET: NavBar/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.NavBar == null)
            {
                return NotFound();
            }

            var navBar = await _context.NavBar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (navBar == null)
            {
                return NotFound();
            }

            return View(navBar);
        }

        // POST: NavBar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.NavBar == null)
            {
                return Problem("Entity set 'ApplicationDbContext.NavBar'  is null.");
            }
            var navBar = await _context.NavBar.FindAsync(id);
            if (navBar != null)
            {
                _context.NavBar.Remove(navBar);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NavBarExists(string id)
        {
          return (_context.NavBar?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
