using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class ServerAppContext : DbContext
    {
        public ServerAppContext (DbContextOptions<ServerAppContext> options)
            : base(options)
        {
        }

        public DbSet<ServerApp.Models.Immunization> Immunization { get; set; }

        public DbSet<ServerApp.Models.Organization> Organization { get; set; }

        public DbSet<ServerApp.Models.Patient> Patient { get; set; }

        public DbSet<ServerApp.Models.Provider> Provider { get; set; }
    }
}
