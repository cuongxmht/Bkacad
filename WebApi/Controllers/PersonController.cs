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
using WebApi.Shared;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PersonController : Controller
    {
        private readonly ILogger<PersonController> _logger;
        private readonly ApplicationDbContext _context;
        public PersonController(ILogger<PersonController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context=context;
        }
        private bool PersonExists(string id)
        {
          return (_context.Person?.Any(e => e.PersonId == id)).GetValueOrDefault();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>?>> GetPersons()
        {
            var model= await _context.Person.ToListAsync();
            if(model==null)
                return NotFound();
            return model;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPersonById(string id)
        {
            var model= await _context.Person.FirstOrDefaultAsync(f=>f.PersonId==id);
            if(model==null)
                return NotFound();
            return model;
        }
        [HttpGet("getbyname/{name}")]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonByName(string name)
        {
            name=Utils.GetInstance().ConvertToUnSign(name).ToLower().Trim();
            var lst=_context.Person?.Where(f=>!string.IsNullOrWhiteSpace(f.NonSignName) 
                && f.NonSignName.ToLower().Contains(name));
            if(lst==null)return NotFound();

            return await lst.ToListAsync();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(string id, Person person)
        {
            if(id!=person.PersonId)
                return BadRequest();
            person.NonSignName=Utils.GetInstance().ConvertToUnSign(person.FullName).Trim();
            _context.Entry(person).State=EntityState.Modified;
            try{
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException){
                if(!PersonExists(id))
                {
                    return NotFound();
                }
                else throw;
            }
            return NoContent();
        }
        [HttpPost()]
        public async Task<ActionResult<Person>> PostPerson(Person person )
        {
            if(_context.Person==null)
            {
                return Problem("Entity set 'ApplicationContext.Person' is null");
            }
            person.NonSignName=Utils.GetInstance().ConvertToUnSign(person.FullName).Trim();
            _context.Person.Add(person);
            try{
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException){
                if(PersonExists(person.PersonId))
                {
                    return Conflict();
                }
                else throw;
            }

            return CreatedAtAction("GetPersonById",new {id=person.PersonId},person);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(string id)
        {
            if(_context.Person==null)
            {
                return NotFound();
            }
            var person=_context.Person.FirstOrDefault(f=>f.PersonId==id);
            if(person==null) return NotFound();
            _context.Person.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}