using System.IO;
using System.Web;
using Nancy.Json;
using Nancy.ModelBinding;
namespace PackagesApi
{
    using Nancy;

    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/test"] = parameters => DataHandler.GetProgramListAsJSON();

            Get["/admin"] = parameter => View["admin.sshtml"];

            Get["/"] = parameters => View["index.sshtml"];

            Post["/update"] = parameters =>
                {
                    var pack = new PackageHandler();

                    pack.ClearPackages();
                    pack.UpdatePackages();

                    return "success";
                };
            

            Post["/list"] = parameters =>
                {
                    var model = this.Bind<Rootobject>();
                    var scriptFilePath = DataHandler.CreateTempScriptFile(model);
                    var serializer = new JavaScriptSerializer();
                    return serializer.Serialize(scriptFilePath);
                };

            Get["/scriptfile/{scriptFile}"] = parameters =>
                {
                    var response = new Response();
                    
                    var path = HttpRuntime.AppDomainAppPath + @"temp\" + parameters.scriptFile;
                    
                    var buffer = DataHandler.CopyFileToBuffer(path);

                    response.Headers.Add("Content-Disposition", "attachment; filename=" + parameters.scriptFile);

                    DataHandler.CreateResponse(response, buffer);
                    
                    File.Delete(path);

                    return response;

                };
        }
    }
}