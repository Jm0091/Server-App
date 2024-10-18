using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;
using ServerApp.Controllers;
using ServerApp;
using System;
using ServerApp.Controllers;
using ServerApp.Models;

namespace ProjectTests
{
    [TestClass]
    public class UnitTest1
    {

        /// <summary>
        /// Test to Pass -  method checks Post method from Patient's Controller
        /// </summary>
        [TestMethod]
        public async Task TestMethod_Post_Patient()
        {
            //arrange
            ServerAppContext _context = CreateContext("index");
            Patient patient = new Patient();
            patient.FirstName = "Bob";
            patient.LastName = "John";
            patient.Id = Guid.NewGuid();
            patient.CreationTime = DateTimeOffset.Now;
            patient.DateOfBirth = new DateTimeOffset(2008, 5, 1, 8, 6, 32, new TimeSpan(1, 0, 0));

            //act
            PatientsController patientController = new PatientsController(_context);
            Task<ActionResult<Patient>> postPatient = patientController.PostPatient(patient);
            await postPatient;
            var p = postPatient.Result.Result;


            Patient database_patient = await _context.Patient.FindAsync(patient.Id);
            //assert
            Assert.AreEqual(database_patient, patient);

        }

        /// <summary>
        /// Test to Pass - method checks delete() from Organization Controller
        /// </summary>
        [TestMethod]
        public async Task TestMethod_Delete_Organization()
        {
            ServerAppContext _context = CreateContext("DeleteCheck");
            OrganizationsController organizationsController = new OrganizationsController(_context);

            Task<ActionResult<Organization>> organizationDelete = organizationsController.DeleteOrganization(Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7"));
            await organizationDelete;

            //getting data of deleted id, it will be null if record with id got deleted
            Organization database_organization = await _context.Organization.FindAsync(Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7"));

            Assert.AreEqual(database_organization, null, "Problem Method 3 - Immunization");
        }

        /// <summary>
        /// Test to Pass - method checks Put() from Provider Controller
        /// </summary>
        [TestMethod]
        public async Task TestMethod_Put_Provider()
        {
            ServerAppContext _context = CreateContext("PutCheck");
            ProvidersController providersController = new ProvidersController(_context);
            Provider provider = await _context.Provider.FindAsync(Guid.Parse("2c9e6679-7425-40de-944b-e07fc1f90ae7"));

            provider.FirstName = "Shila";           //updating record
            _context.Entry(provider).State = EntityState.Modified;

            await providersController.PutProvider(Guid.Parse("2c9e6679-7425-40de-944b-e07fc1f90ae7"), provider);

            Provider database_organization = await _context.Provider.FindAsync(Guid.Parse("2c9e6679-7425-40de-944b-e07fc1f90ae7"));

            Assert.AreEqual(database_organization.FirstName, "Shila", "Problem Method 3 - Immunization");
        }

        /// <summary>
        /// Test to Pass - method checks Get() with parameter of officialName = 'Covid-19' from Immunization's Controller
        /// </summary>
        [TestMethod]
        public async Task TestMethod_Get_Immunization()
        {
            ServerAppContext _context = CreateContext("getCheck");

            ImmunizationsController immunizationsController = new ImmunizationsController(_context);
            Immunization immunization = await _context.Immunization.FindAsync(Guid.Parse("2c9e6679-7425-40de-944b-e07fc1f90ae7"));

            string officialName = "Covid-19";
            Task<ActionResult<List<Immunization>>> listImmunizedItems = immunizationsController.Get(null, officialName, null, null);
            await listImmunizedItems;
            var listValue = listImmunizedItems.Result.Result;

            Assert.AreEqual(listValue, immunization, "Problem Method - Immunization");
        }


        /// <summary>
        ///  Creating database in memory for testing purpose 
        /// </summary>
        /// <param name="databaseName">Temporary Database name</param>
        /// <returns>context</returns>
        private ServerAppContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ServerAppContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
            var context = new ServerAppContext(options);

            context.Patient.AddRange(
                new Patient()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Wick",
                    CreationTime = DateTimeOffset.Now,
                    DateOfBirth = new DateTimeOffset(2008, 5, 1, 8, 6, 32, new TimeSpan(1, 0, 0))
                }
            );
            context.Immunization.AddRange(
                new Immunization()
                {
                    Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae8"),
                    OfficialName = "Covid-19",
                    TradeName = "Dose 1",
                    LotNumber = "123",
                    ExpirationDate = new DateTimeOffset(2023, 12, 12, 8, 6, 32, new TimeSpan(1, 0, 0)),
                    CreationTime = new DateTimeOffset(2018, 5, 1, 8, 6, 32, new TimeSpan(1, 0, 0)),
                    UpdatedTime = new DateTimeOffset(2021, 5, 7, 8, 7, 32, new TimeSpan(1, 0, 0))
                }
            );
            context.Organization.AddRange(
                new Organization()
                {
                    Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7"),
                    Name = "Canada Org.",
                    Type = "Self",
                    Address = "Canada",
                    CreationTime = new DateTimeOffset(2020, 11, 4, 8, 6, 32, new TimeSpan(1, 0, 0)),
                }
            );
            context.Provider.AddRange(
                new Provider()
                {
                    Id = Guid.Parse("2c9e6679-7425-40de-944b-e07fc1f90ae7"),
                    FirstName = "HomoLo",
                    LastName = "Mark",
                    Address = "998",
                    LicenseNumber = uint.Parse("123"),
                    CreationTime = new DateTimeOffset(2020, 11, 4, 8, 6, 32, new TimeSpan(1, 0, 0)),
                }
            );
            context.SaveChanges();
            return context;
        }
    }
}
