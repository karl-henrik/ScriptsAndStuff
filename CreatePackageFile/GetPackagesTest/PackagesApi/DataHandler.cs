using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GetPackagesTest.Context;
using System.Runtime.Serialization.Formatters.Binary;
using Nancy;
using Nancy.Json;


namespace PackagesApi
{
    public class DataHandler
    {
        public static dynamic GetProgramListAsJSON()
        {
            using (var db = new ProgramContext())
            {
                var programs = from p in db.Programs
                               select new
                                   {
                                       Name = p.ProgramName
                                   };
                var serializer = new JavaScriptSerializer();
                return serializer.Serialize(programs);

            }
        }

        public static Response CreatePackageFileFromJSON(Rootobject model)
        {

            var response = new Response();

            response.Headers.Add("Content-Disposition", "attachment; filename=test.txt");
            response.ContentType = "text/plain";
            response.Contents = stream =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var package in model.packages)
                    {
                        writer.Write(package.package);
                    }
                }
            };

            return response;
            
        }
    }
}