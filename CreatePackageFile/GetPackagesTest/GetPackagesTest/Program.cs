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
            var ser = new XmlSerializer(typeof(XMLClasses.feed));

            var link = "http://chocolatey.org/api/v2/Packages";
            
            using (var db = new ProgramContext())
            {
                while (link != null && link.Length > 15)
                {
                    XMLClasses.feed mainfeed;

                    mainfeed = ReadFromMainfeed(link, ser);

                    foreach (var item in mainfeed.Items.Where(x => x.GetType() == typeof(XMLClasses.feedEntry)))
                    {
                        var val = item as XMLClasses.feedEntry;

                        if (val != null)
                        {

                            var queryProgram = GetStoredProgramPackagesFromDatabase(db, val);

                            var queryProgramVersion = GetStoredProgramPackageVersionsFromDatabase(db, val);

                            if (!queryProgram.Any())
                            {
                                AddProgramPackageToDatabase(val, db);
                            }
                            else if (!queryProgramVersion.Any())
                            {
                                AddProgramPackageVersionToDatabase(queryProgram, db, val);
                            }

                            db.SaveChanges();
                        }

                    }

                    var feedLink = mainfeed.Items.Last(x => x.GetType() == typeof(XMLClasses.feedLink)) as XMLClasses.feedLink;

                    link = feedLink != null ? feedLink.href : null;
                }

            }
            Console.ReadLine();
        }

        /// <summary>
        /// Access the main RSS feed of Chocolatey, and return as XML object
        /// </summary>
        /// <param name="link">The address of the RSS feed</param>
        /// <param name="ser">The XML Serializer</param>
        /// <returns></returns>
        private static XMLClasses.feed ReadFromMainfeed(string link, XmlSerializer ser)
        {
            XMLClasses.feed mainfeed;
            using (XmlReader reader = new XmlTextReader(link))
            {
                mainfeed = (XMLClasses.feed) ser.Deserialize(reader);
            }
            return mainfeed;
        }


        /// <summary>
        /// Add Program Package Version To the Database via Entityframework. 
        /// The Program package version is allways linked to a Program package.
        /// </summary>
        /// <param name="queryProgram"></param>
        /// <param name="db"></param>
        /// <param name="val"></param>
        private static void AddProgramPackageVersionToDatabase(IOrderedQueryable<EF_Program> queryProgram, ProgramContext db, XMLClasses.feedEntry val)
        {
            var program = queryProgram.First();
            db.ProgramVersions.Add(new EF_ProgramVersion() {Program = program, Version = val.properties.Version});
        }

        /// <summary>
        /// Add a program package to the database via Entityframework
        /// </summary>
        /// <param name="val"></param>
        /// <param name="db"></param>
        private static void AddProgramPackageToDatabase(XMLClasses.feedEntry val, ProgramContext db)
        {
            var program = new EF_Program {ProgramName = val.title.Value};

            if (program.ProgramVersions == null)
                program.ProgramVersions = new List<EF_ProgramVersion>();

            program.ProgramVersions.Add(new EF_ProgramVersion() {Program = program, Version = val.properties.Version});

            db.Programs.Add(program);
        }

        private static IQueryable<EF_ProgramVersion> GetStoredProgramPackageVersionsFromDatabase(ProgramContext db, XMLClasses.feedEntry val)
        {
            var queryProgramVersion = from p in db.ProgramVersions
                                      where p.Version == val.properties.Version
                                      select p;
            return queryProgramVersion;
        }

        private static IOrderedQueryable<EF_Program> GetStoredProgramPackagesFromDatabase(ProgramContext db, XMLClasses.feedEntry val)
        {
            var queryProgram = from p in db.Programs
                               where p.ProgramName == val.title.Value
                               orderby p.ProgramName
                               select p;
            return queryProgram;
        }
    }
}
