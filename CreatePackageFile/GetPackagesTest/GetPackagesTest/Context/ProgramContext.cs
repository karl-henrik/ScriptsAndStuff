using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetPackagesTest.Models.EF;

namespace GetPackagesTest.Context
{
    class ProgramContext : DbContext
    {
        public DbSet<EF_Program> Programs { get; set; }
        public DbSet<EF_ProgramVersion> ProgramVersions { get; set; }
    }
}
