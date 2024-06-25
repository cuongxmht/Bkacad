using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VicemMvcIdentity.Data;
using VicemMvcIdentity.Models.Entities;

namespace VicemMvcIdentity.Controllers
{
    public class MemberUnitController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberUnitController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MemberUnit
        public async Task<IActionResult> Index()
        {
            return View(await _context.MemberUnit.ToListAsync());
        }

        // GET: MemberUnit/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberUnit = await _context.MemberUnit
                .FirstOrDefaultAsync(m => m.MemberUnitId == id);
            if (memberUnit == null)
            {
                return NotFound();
            }

            return View(memberUnit);
        }

        // GET: MemberUnit/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MemberUnit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberUnitId,Name,Address,PhoneNumber,WebsiteUrl")] MemberUnit memberUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(memberUnit);
        }

        // GET: MemberUnit/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberUnit = await _context.MemberUnit.FindAsync(id);
            if (memberUnit == null)
            {
                return NotFound();
            }
            return View(memberUnit);
        }

        // POST: MemberUnit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberUnitId,Name,Address,PhoneNumber,WebsiteUrl")] MemberUnit memberUnit)
        {
            if (id != memberUnit.MemberUnitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberUnitExists(memberUnit.MemberUnitId))
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
            return View(memberUnit);
        }

        // GET: MemberUnit/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberUnit = await _context.MemberUnit
                .FirstOrDefaultAsync(m => m.MemberUnitId == id);
            if (memberUnit == null)
            {
                return NotFound();
            }

            return View(memberUnit);
        }

        // POST: MemberUnit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberUnit = await _context.MemberUnit.FindAsync(id);
            if (memberUnit != null)
            {
                _context.MemberUnit.Remove(memberUnit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberUnitExists(int id)
        {
            return _context.MemberUnit.Any(e => e.MemberUnitId == id);
        }
    }
}
