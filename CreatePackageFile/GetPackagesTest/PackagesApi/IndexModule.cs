using System.Collections.Generic;
using System.IO;
using Nancy.ModelBinding;

namespace PackagesApi
{
    using Nancy;

    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/test"] = parameters =>
                {
                    return DataHandler.GetProgramListAsJSON();
                };

            Get["/"] = parameters => View["index.sshtml"];

            Post["/list"] = parameters =>
                {
                    var model = this.Bind<Rootobject>();
                    var test = DataHandler.CreatePackageFileFromJSON(model);

                    return "Grinch";

                };
        }
    }

    
}