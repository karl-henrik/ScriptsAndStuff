using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetPackagesTest.Models.EF
{
    public class EF_Program
    {
        [Key]
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }

        public virtual List<EF_ProgramVersion> ProgramVersions { get; set; }
    }

    public class EF_ProgramVersion
    {
        [Key]
        public int VersionId { get; set; }
        public string Version { get; set; }

        public virtual EF_Program Program { get; set; }
    }
}
