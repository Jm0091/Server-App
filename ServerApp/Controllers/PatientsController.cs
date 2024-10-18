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
using System.Net;
/// <summary>
/// This Project is done in group by Yash Chaudhary (000820480) and Jems Chaudhary (000814314). This is our own work and not shared to anyone else.
/// </summary>
namespace ServerApp.Controllers
{
    /// <summary>
    /// Controller for Patient Model Class
    /// </summary>
    [Route("api/Patient")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly ServerAppContext _context;

        public PatientsController(ServerAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method provides records for any following parameters
        /// </summary>
        /// <param name="firstName">Firstname of Patient></param>
        /// <param name="lastName">Lasstname of Patient></param>
        /// <param name="dateOfBirth">dateOfBirth of  Patient></param>
        /// <returns>returns records matching to provided value</returns>
        [HttpGet()]
        public async Task<ActionResult<List<Patient>>> Get(string? firstName, string? lastName,DateTimeOffset? dateOfBirth)
        {
            ErrorMessage e;
            IQueryable<Patient> patient = null;
            if (firstName == null && lastName == null && dateOfBirth == null)
            {
                e = new ErrorMessage(404, "Not found");
                return CreateError(e);
            }
            else if (firstName != null)
            {
                firstName = firstName.ToLower();
                patient = from p in _context.Patient
                          where p.FirstName.ToLower().Equals(firstName)
                          select p;
            }
            else if (lastName != null)
            {
                lastName = lastName.ToLower();
                patient = from p in _context.Patient
                          where p.LastName.ToLower().Equals(lastName)
                          select p;
            }
            else if (dateOfBirth != null)
            {
                patient = from p in _context.Patient
                          where p.DateOfBirth.Date.Equals(((DateTimeOffset)dateOfBirth).Date)
                          select p;
            }
            return await patient.ToListAsync();           
        }

        /// <summary>
        /// GET: api/Patient/5
        /// </summary>
        /// <param name="id">Patient id to lookup</param>
        /// <returns>Patient object if exists in the database</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(Guid id)
        {
            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                var error = new ErrorMessage(404,$"Patient doesn't exist with id: {id}");
                return CreateError(error);
            }
            return patient;
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(Guid id, Patient patient)
        {
            if (id != patient.Id)
            {
                var error = new ErrorMessage(400, $"id in the uri doesn't match the id in patient obeject provided in body");
                return CreateError(error);
            }
            if (!ModelState.IsValid) {
                var error = new ErrorMessage(400, $"Patient provided is not valid");
                return CreateError(error);
            }

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    var error = new ErrorMessage(404, $"can't update. Patient doesn't exist with id: {id}");
                    return CreateError(error);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Patients
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            if (!ModelState.IsValid) {
                var error = new ErrorMessage(400, $"Patient provided is not valid");
                return CreateError(error);
            }
            _context.Patient.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatient", new { id = patient.Id }, patient);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> DeletePatient(Guid id)
        {
            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                var error = new ErrorMessage(404, $"Patient doesn't exist with id: {id}");
                return CreateError(error);
            }

            _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();

            return patient;
        }
        //checking id exists or not
        private bool PatientExists(Guid id)
        {
            return _context.Patient.Any(e => e.Id == id);
        }
        /// <summary>
        /// CreateError used for return error message
        /// </summary>
        /// <param name="e">Getting error message to log in database</param>
        /// <returns>Serialized Error message with statuscode, message and its id</returns>
        private ContentResult CreateError(ErrorMessage e) {
            HttpContext.Response.StatusCode = e.StatusCode;
            var result = Content(e.ToString(), "application/json; charset=utf-8");
            return result;
        }
    }
}
