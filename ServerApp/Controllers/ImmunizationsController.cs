using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;
/// <summary>
/// This Project is done in group by Yash Chaudhary (000820480) and Jems Chaudhary (000814314). This is our own work and not shared to anyone else.
/// </summary>
namespace ServerApp.Controllers
{
    /// <summary>
    /// Controller for Immunization Model Class
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImmunizationsController : ControllerBase
    {
        private readonly ServerAppContext _context;

        public ImmunizationsController(ServerAppContext context)
        {
            _context = context;
        }

        
        // GET: api/Immunizations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Immunization>> GetImmunization(Guid id)
        {
            var immunization = await _context.Immunization.FindAsync(id);

            if (immunization == null)
            {
                var error = new ErrorMessage(404, $"Immunization doesn't exist with id: {id}");
                return CreateError(error);
            }

            return immunization;
        }

        // PUT: api/Immunizations/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImmunization(Guid id, Immunization immunization)
        {
            if (id != immunization.Id)
            {
                var error = new ErrorMessage(400, $"id in the uri doesn't match the id in Immunization obeject provided in body");
                return CreateError(error);
            }
            if (!ModelState.IsValid)
            {
                var error = new ErrorMessage(400, $"Immunization provided is not valid");
                return CreateError(error);
            }

            immunization.UpdatedTime = DateTimeOffset.Now;
            _context.Entry(immunization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImmunizationExists(id))
                {
                    var error = new ErrorMessage(404, $"can't update. Immunization doesn't exist with id: {id}");
                    return CreateError(error);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Immunizations
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Immunization>> PostImmunization(Immunization immunization)
        {
            if (!ModelState.IsValid)
            {
                var error = new ErrorMessage(400, $"Immunization provided is not valid");
                return CreateError(error);
            }
            _context.Immunization.Add(immunization);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetImmunization", new { id = immunization.Id }, immunization);
        }

        // DELETE: api/Immunizations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Immunization>> DeleteImmunization(Guid id)
        {
            var immunization = await _context.Immunization.FindAsync(id);
            if (immunization == null)
            {
                var error = new ErrorMessage(404, $"Delete doesn't exist with id: {id}");
                return CreateError(error);
            }

            _context.Immunization.Remove(immunization);
            await _context.SaveChangesAsync();

            return immunization;
        }

        private bool ImmunizationExists(Guid id)
        {
            return _context.Immunization.Any(e => e.Id == id);
        }

        /// <summary>
        /// Method provides records for any following parameters
        /// </summary>
        /// <param name="tradeName">tradeName of Immunization></param>
        /// <param name="officialName">officialName of Immunization></param>
        /// <param name="lotNum">lotNum of Immunization></param>
        /// <param name="creationTime">creationTime of Immunization></param>
        /// <returns>returns records matching to provided value</returns> 
        [HttpGet()]
        public async Task<ActionResult<List<Immunization>>> Get(string? tradeName, string? officialName, string? lotNum, DateTimeOffset? creationTime)
        {
            ErrorMessage e;
            IQueryable<Immunization> immunizations = null;
            if (tradeName == null && officialName == null && lotNum == null && creationTime == null)
            {
                e = new ErrorMessage(404, "Not found");
                return CreateError(e);
            }
            else if (tradeName != null)
            {
                tradeName = tradeName.ToLower();
                immunizations = from p in _context.Immunization
                          where p.TradeName.ToLower().Equals(tradeName)
                          select p;
            }
            else if (officialName != null)
            {
                officialName = officialName.ToLower();
                immunizations = from p in _context.Immunization
                          where p.OfficialName.ToLower().Equals(officialName)
                          select p;
            }
            else if (lotNum != null)
            {
                lotNum = lotNum.ToLower();
                immunizations = from p in _context.Immunization
                                where p.LotNumber.ToLower().Equals(lotNum)
                                select p;
            }
            else if (creationTime != null)
            {
                immunizations = from p in _context.Immunization
                          where p.CreationTime.Date.Equals(((DateTimeOffset)creationTime).Date)
                          select p;
            }
            return await immunizations.ToListAsync();
        }
        /// <summary>
        /// CreateError used for return error message
        /// </summary>
        /// <param name="e">Getting error message to log in database</param>
        /// <returns>Serialized Error message with statuscode, message and its id</returns>
        private ContentResult CreateError(ErrorMessage e)
        {
            HttpContext.Response.StatusCode = e.StatusCode;
            var result = Content(e.ToString(), "application/json; charset=utf-8");
            return result;
        }
    }
}
