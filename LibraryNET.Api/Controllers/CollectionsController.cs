using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryNET.Database;
using LibraryNET.Model;

namespace LibraryNET.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        private readonly DbConnection _context;

        public CollectionsController(DbConnection context)
        {
            _context = context;
        }

        // GET: api/Collections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Collection>>> GetfpCollection()
        {
            return await _context.fpCollection.ToListAsync();
        }

        // GET: api/Collections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Collection>> GetCollection(int id)
        {
            var collection = await _context.fpCollection.FindAsync(id);

            if (collection == null)
            {
                return NotFound();
            }

            return collection;
        }

        // PUT: api/Collections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollection(int id, Collection collection)
        {
            if (id != collection.Id)
            {
                return BadRequest();
            }

            _context.Entry(collection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(id))
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

        // POST: api/Collections
        [HttpPost]
        public async Task<ActionResult<Collection>> PostCollection(Collection collection)
        {
            _context.fpCollection.Add(collection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCollection", new { id = collection.Id }, collection);
        }

        // DELETE: api/Collections/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Collection>> DeleteCollection(int id)
        {
            var collection = await _context.fpCollection.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            _context.fpCollection.Remove(collection);
            await _context.SaveChangesAsync();

            return collection;
        }

        private bool CollectionExists(int id)
        {
            return _context.fpCollection.Any(e => e.Id == id);
        }
    }
}
