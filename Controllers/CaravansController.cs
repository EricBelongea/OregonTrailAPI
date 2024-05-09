using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OregonTrailAPI.Context.Models;

namespace OregonTrailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaravansController : ControllerBase
    {
        private readonly CaravanContext _context;

        public CaravansController(CaravanContext context)
        {
            _context = context;
        }

        // GET: api/Caravans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Caravan>>> GetCaravans()
        {
            return await _context.Caravans
                .Include(c => c.Wagons)
                .ToListAsync();
        }

        // GET: api/Caravans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Caravan>> GetCaravan(int id)
        {
            var caravan = await _context.Caravans
                .Include(c => c.Wagons)
                .ThenInclude(w => w.Passengers)
                .FirstOrDefaultAsync(c => c.CaravanId == id);

            if (caravan == null)
            {
                return NotFound();
            }

            return caravan;
        }

        // PUT: api/Caravans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCaravan(int id, Caravan caravan)
        {
            if (id != caravan.CaravanId)
            {
                return BadRequest();
            }

            _context.Entry(caravan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaravanExists(id))
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

        // POST: api/Caravans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Caravan>> PostCaravan(Caravan caravan)
        {
            _context.Caravans.Add(caravan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCaravan", new { id = caravan.CaravanId }, caravan);
        }

        // DELETE: api/Caravans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCaravan(int id)
        {
            var caravan = await _context.Caravans.FindAsync(id);
            if (caravan == null)
            {
                return NotFound();
            }

            _context.Caravans.Remove(caravan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CaravanExists(int id)
        {
            return _context.Caravans.Any(e => e.CaravanId == id);
        }
    }
}
