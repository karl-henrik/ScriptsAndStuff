using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GetPackagesTest.Context;
using GetPackagesTest.Models.EF;

namespace GetPackagesTest
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlSerializer ser = new XmlSerializer(typeof(XMLClasses.feed));

            var mainfeed = new XMLClasses.feed();

            var link = "http://chocolatey.org/api/v2/Packages";


            using (var db = new ProgramContext())
            {



                while (link != null && link.Length > 15)
                {
                    using (XmlReader reader = new XmlTextReader(link))
                    {
                        mainfeed = (XMLClasses.feed)ser.Deserialize(reader);
                    }

                    foreach (var item in mainfeed.Items.Where(x => x.GetType() == typeof(XMLClasses.feedEntry)))
                    {
                        var val = item as XMLClasses.feedEntry;

                        if (val != null)
                        {

                            var queryProgram = from p in db.Programs
                                               where p.ProgramName == val.title.Value
                                               orderby p.ProgramName
                                               select p;

                            var queryProgramVersion = from p in db.ProgramVersions
                                                      where p.Version == val.properties.Version
                                                      select p;

                            if (!queryProgram.Any())
                            {
                                var program = new EF_Program {ProgramName = val.title.Value};

                                if(program.ProgramVersions == null)
                                    program.ProgramVersions = new List<EF_ProgramVersion>();

                                program.ProgramVersions.Add(new EF_ProgramVersion() {Program = program,Version = val.properties.Version});
                                
                                db.Programs.Add(program);

                            }
                            else if (!queryProgramVersion.Any())
                            {
                                var program = queryProgram.First();
                                db.ProgramVersions.Add(new EF_ProgramVersion() { Program = program, Version = val.properties.Version });
                            }


                            db.SaveChanges();
                            Console.WriteLine(val.title.Value);
                            Console.WriteLine(val.properties.Version);
                        }

                    }

                    var feedLink = mainfeed.Items.Last(x => x.GetType() == typeof(XMLClasses.feedLink)) as XMLClasses.feedLink;

                    link = feedLink != null ? feedLink.href : null;
                }

            }




            Console.ReadLine();
        }




    }
}
