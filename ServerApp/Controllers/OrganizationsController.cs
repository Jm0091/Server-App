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
    /// Controller for Organization Model Class
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly ServerAppContext _context;

        public OrganizationsController(ServerAppContext context)
        {
            _context = context;
        }

        // GET: api/Organizations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetOrganization(Guid id)
        {
            var organization = await _context.Organization.FindAsync(id);

            if (organization == null)
            {
                var error = new ErrorMessage(404, $"Organization doesn't exist with id: {id}");
                return CreateError(error);
            }

            return organization;
        }

        // PUT: api/Organizations/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrganization(Guid id, Organization organization)
        {
            if (id != organization.Id)
            {
                var error = new ErrorMessage(400, $"id in the uri doesn't match the id in Organization obeject provided in body");
                return CreateError(error);
            }
            if (!ModelState.IsValid)
            {
                var error = new ErrorMessage(400, $"Organization provided is not valid");
                return CreateError(error);
            }

            _context.Entry(organization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationExists(id))
                {
                    var error = new ErrorMessage(404, $"can't update. Organization doesn't exist with id: {id}");
                    return CreateError(error);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Organizations
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Organization>> PostOrganization(Organization organization)
        {
            if (!ModelState.IsValid)
            {
                var error = new ErrorMessage(400, $"Organization provided is not valid");
                return CreateError(error);
            }
            _context.Organization.Add(organization);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrganization", new { id = organization.Id }, organization);
        }

        // DELETE: api/Organizations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Organization>> DeleteOrganization(Guid id)
        {
            var organization = await _context.Organization.FindAsync(id);
            if (organization == null)
            {
                var error = new ErrorMessage(404, $"Organization doesn't exist with id: {id}");
                return CreateError(error);
            }

            _context.Organization.Remove(organization);
            await _context.SaveChangesAsync();

            return organization;
        }

        private bool OrganizationExists(Guid id)
        {
            return _context.Organization.Any(e => e.Id == id);
        }

        /// <summary>
        /// Method provides records for any following parameters
        /// </summary>
        /// <param name="name">name of Organization></param>
        /// <param name="type">type of Organization></param>
        /// <returns>returns records matching to provided value</returns> 
        [HttpGet()]
        public async Task<ActionResult<List<Organization>>> Get(string? name, string? type)
        {
            ErrorMessage e;
            IQueryable<Organization> organization = null;
            if (name == null && type == null)
            {
                e = new ErrorMessage(404, "Not found");
                return CreateError(e);
            }
            else if (name != null)
            {
                name = name.ToLower();
                organization = from p in _context.Organization
                          where p.Name.ToLower().Equals(name)
                          select p;
            }
            else if (type != null)
            {
                type = type.ToLower();
                organization = from p in _context.Organization
                          where p.Type.ToLower().Equals(type)
                          select p;
            }

            return await organization.ToListAsync();
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
