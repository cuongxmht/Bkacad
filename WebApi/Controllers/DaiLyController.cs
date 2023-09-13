using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaiLyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DaiLyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DaiLy
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<DaiLy>>> GetDaiLies()
        {
          if (_context.DaiLies == null)
          {
              return NotFound();
          }
            return await _context.DaiLies.ToListAsync();
        }

        // GET: api/DaiLy/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DaiLy>> GetDaiLy(string id)
        {
          if (_context.DaiLies == null)
          {
              return NotFound();
          }
            var daiLy = await _context.DaiLies.FindAsync(id);

            if (daiLy == null)
            {
                return NotFound();
            }

            return daiLy;
        }
        [HttpGet("gethethongdaily")]
        public async Task<ActionResult<IEnumerable<HeThongDaiLy>>> GetHethongDaiLy()
        {
            var htdl=await (from dl in _context.DaiLies
                            join pp in _context.HeThongPhanPhois
                            on dl.MaHTPP equals pp.MaHTPP
                            select new HeThongDaiLy{
                                MaDaiLy=dl.MaDaiLy,
                                TenDaiLy=dl.TenDaiLy,
                                MaHTPP=pp.MaHTPP,
                                TenHTPP=pp.TenHTPP
                            }).ToListAsync();

            return htdl;
        }

        // PUT: api/DaiLy/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDaiLy(string id, DaiLy daiLy)
        {
            if (id != daiLy.MaDaiLy)
            {
                return BadRequest();
            }

            _context.Entry(daiLy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DaiLyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DaiLy
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DaiLy>> PostDaiLy(DaiLy daiLy)
        {
          if (_context.DaiLies == null)
          {
              return Problem("Entity set 'ApplicationDbContext.DaiLies'  is null.");
          }
            _context.DaiLies.Add(daiLy);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DaiLyExists(daiLy.MaDaiLy))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDaiLy", new { id = daiLy.MaDaiLy }, daiLy);
        }

        // DELETE: api/DaiLy/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDaiLy(string id)
        {
            if (_context.DaiLies == null)
            {
                return NotFound();
            }
            var daiLy = await _context.DaiLies.FindAsync(id);
            if (daiLy == null)
            {
                return NotFound();
            }

            _context.DaiLies.Remove(daiLy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DaiLyExists(string id)
        {
            return (_context.DaiLies?.Any(e => e.MaDaiLy == id)).GetValueOrDefault();
        }
    }
}
