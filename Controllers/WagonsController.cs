using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OregonTrailAPI.Context.Models;

namespace OregonTrailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WagonsController : ControllerBase
    {
        private readonly CaravanContext _context;

        public WagonsController(CaravanContext context)
        {
            _context = context;
        }

        // GET: api/Wagons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wagon>>> GetWagons()
        {
            return await _context.Wagons.ToListAsync();
        }

        // GET: api/Wagons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wagon>> GetWagon(int id)
        {
            var wagon = await _context.Wagons
                .Include(w => w.Passengers)
                    .ThenInclude(p => p.PassengerFoods)
                        .ThenInclude(navigationPropertyPath: pf => pf.Food)
                .Include(w => w.Items)
                .FirstOrDefaultAsync(W => W.WagonId == id);

            if (wagon == null)
            {
                return NotFound();
            }

            return wagon;
        }

        // PUT: api/Wagons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWagon(int id, Wagon wagon)
        {
            if (id != wagon.WagonId)
            {
                return BadRequest();
            }

            _context.Entry(wagon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WagonExists(id))
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

        // POST: api/Wagons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Wagon>> PostWagon(Wagon wagon)
        {
            _context.Wagons.Add(wagon);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWagon", new { id = wagon.WagonId }, wagon);
        }

        // DELETE: api/Wagons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWagon(int id)
        {
            var wagon = await _context.Wagons.FindAsync(id);
            if (wagon == null)
            {
                return NotFound();
            }

            _context.Wagons.Remove(wagon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WagonExists(int id)
        {
            return _context.Wagons.Any(e => e.WagonId == id);
        }
    }
}
