using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly ApplicationDbContext _context;
        public EmployeeController(ILogger<EmployeeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context=context;
        }
        private bool EmpExists(string id)
        {
          return (_context.Employees?.Any(e => e.PersonId == id)).GetValueOrDefault();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>?>> GetEmps()
        {
            var model= await _context.Employees.ToListAsync();
            if(model==null)
                return NotFound();
            return model;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmpById(string id)
        {
            var model= await _context.Employees.FirstOrDefaultAsync(f=>f.EmployeeId==id);
            if(model==null)
                return NotFound();
            return model;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmp(string id, Employee emp)
        {
            if(id!=emp.EmployeeId)
                return BadRequest();
            _context.Entry(emp).State=EntityState.Modified;
            try{
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException){
                if(!EmpExists(id))
                {
                    return NotFound();
                }
                else throw;
            }
            return NoContent();
        }
        [HttpPost()]
        public async Task<ActionResult<Employee>> PostEmp(Employee emp )
        {
            if(_context.Employees==null)
            {
                return Problem("Entity set 'ApplicationContext.Person' is null");
            }
            _context.Person.Add(emp);
            try{
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException){
                if(EmpExists(emp.EmployeeId))
                {
                    return Conflict();
                }
                else throw;
            }

            return CreatedAtAction("GetEmpById",new {id=emp.EmployeeId},emp);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmp(string id)
        {
            if(_context.Employees==null)
            {
                return NotFound();
            }
            var employee=_context.Employees.FirstOrDefault(f=>f.EmployeeId==id);
            if(employee==null) return NotFound();
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}