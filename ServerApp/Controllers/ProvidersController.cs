using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;
using System.Text.Json;
/// <summary>
/// This Project is done in group by Yash Chaudhary (000820480) and Jems Chaudhary (000814314). This is our own work and not shared to anyone else.
/// </summary>
namespace ServerApp.Controllers
{
    /// <summary>
    /// Controller for Provider Model Class
    /// </summary>
    [Route("api/Provider")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly ServerAppContext _context;

        public ProvidersController(ServerAppContext context)
        {
            _context = context;
        }

        // GET: api/Providers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Provider>> GetProvider(Guid id)
        {
            var provider = await _context.Provider.FindAsync(id);
            if (provider == null)
            {
                var error = new ErrorMessage(404, $"Provider doesn't exist with id: {id}");
                return CreateError(error);
            }
            
            return provider;
        }

        /// <summary>
        /// Method provides records for any following parameters
        /// </summary>
        /// <param name="firstName">Firstname of Provider</param>
        /// <param name="lastName">Lasstname of Provider</param>
        /// <param name="licenseNumber">Lincense Number</param>
        /// <returns>returns records matching to provided value</returns>
        [HttpGet()]
        public async Task<ActionResult<List<Provider>>> Get(string? firstName, string? lastName, uint? licenseNumber)
        {
            ErrorMessage e;
            IQueryable<Provider> provider = null;
            if (firstName == null && lastName == null && licenseNumber == null)
            {
                e = new ErrorMessage(404, "resource Not found");
                return CreateError(e);
            }
            else if (firstName != null)
            {
                firstName = firstName.ToLower();
                provider = from p in _context.Provider
                          where p.FirstName.ToLower().Equals(firstName)
                          select p;
            }
            else if (lastName != null)
            {
                lastName = lastName.ToLower();
                provider = from p in _context.Provider
                          where p.LastName.ToLower().Equals(lastName)
                          select p;
            }
            else
            {
                provider = from p in _context.Provider
                          where p.LicenseNumber.Equals(licenseNumber)
                          select p;
            }
            return await provider.ToListAsync();
        }

        // PUT: api/Providers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProvider(Guid id, Provider provider)
        {
            if (id != provider.Id)
            {
                var error = new ErrorMessage(400, $"id in the uri doesn't match the id in provider obeject provided in body");
                return CreateError(error);
            }
            if (!ModelState.IsValid)
            {
                var error = new ErrorMessage(400, $"provider provided is not valid");
                return CreateError(error);
            }

            _context.Entry(provider).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProviderExists(id))
                {
                    var error = new ErrorMessage(404, $"can't update. Provider doesn't exist with id: {id}");
                    return CreateError(error);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Providers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Provider>> PostProvider(Provider provider)
        {
            if (!ModelState.IsValid)
            {
                var error = new ErrorMessage(400, $"Provider provided is not valid");
                return CreateError(error);
            }
            _context.Provider.Add(provider);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProvider", new { id = provider.Id }, provider);
        }

        // DELETE: api/Providers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Provider>> DeleteProvider(Guid id)
        {
            var provider = await _context.Provider.FindAsync(id);
            if (provider == null)
            {
                var error = new ErrorMessage(404, $"Provider doesn't exist with id: {id}");
                return CreateError(error);
            }

            _context.Provider.Remove(provider);
            await _context.SaveChangesAsync();

            return provider;
        }
        //checking id exists or not
        private bool ProviderExists(Guid id)
        {
            return _context.Provider.Any(e => e.Id == id);
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
