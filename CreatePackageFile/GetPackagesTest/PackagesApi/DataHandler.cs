using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using GetPackagesTest.Context;
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
            return null;
        }

        public static object CreateTempScriptFile(Rootobject model)
        {
            var path = "ChockIS" + DateTime.Now.Ticks + ".ps1";

            using (var file = new StreamWriter(HttpRuntime.AppDomainAppPath + @"temp\" + path))
            {
                foreach (var package in model.packages)
                {
                    file.WriteLine("cinstm " + package.package);
                }
                file.Flush();
                file.Close();
            }

            return path;
        }

        public static void CreateResponse(Response response, byte[] buffer)
        {

            response.ContentType = "text/plain";
            response.Contents = stream =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    int row = 0;
                    while (ReadLine(buffer, row) != "")
                    {
                        writer.Write(ReadLine(buffer, row++));
                    }
                }
            };
        }

        private static string ReadLine(IEnumerable<byte> buffer, int row)
        {
            int brCount = 0;
            var returnValue = string.Empty;

            foreach (var b in buffer)
            {

                if (brCount == row)
                {
                    returnValue += (char)b;
                }
                if (b == '\n')
                {
                    brCount++;
                }
            }

            return returnValue;

        }

        public static byte[] CopyFileToBuffer(dynamic path)
        {
            byte[] buffer;

            using (FileStream fileStream = File.OpenRead(path))
            {
                buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
            }
            return buffer;
        }
    }
}