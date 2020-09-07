using Structurizr;

namespace Sepes.StructurizrDocs.Models.Containers
{
    public static class SepesContainerFactory
    {      

        public static Container CreateAndAddSepesFrontEnd(SoftwareSystem system)
        {
            var singlePageApplication = system.AddContainer("Single-Page Application", "Provides all of the Sepes functionality to users via their web browser.", "React");          
            return singlePageApplication;
        }

        public static Container CreateAndAddSepesApi(SoftwareSystem system)
        {
            var apiApplication = system.AddContainer("API", "Provides Sepes functionality via a JSON/HTTPS API.", "ASP.NET Core");
            apiApplication.AddTags(Constants.SepesSystemTag);
            return apiApplication;
        }

        public static Container CreateAndAddSepesWorker(SoftwareSystem system)
        {
            var worker = system.AddContainer("Worker", "Creates and monitors cloud resources in Azure.", ".NET Core Azure function");
            worker.AddTags(Constants.SepesSystemTag);
            return worker;
        }

        public static Container CreateAndAddSepesDb(SoftwareSystem softwareSystem)
        {
            var db = softwareSystem.AddContainer("Sepes Database", "Stores studies, sandboxes, user data etc.", "Azure SQL Server");
            db.AddTags(Constants.DatabaseTag);
            return db;
        }
    }
}
