using Structurizr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.StructurizrDocs.Models.Containers
{
    public static class SepesContainerFactory
    {      

        public static Container CreateSepesFrontEnd(SoftwareSystem system)
        {
            var singlePageApplication = system.AddContainer("Single-Page Application", "Provides all of the Sepes functionality to users via their web browser.", "JavaScript and React");          
            return singlePageApplication;
        }

        public static Container CreateSepesApi(SoftwareSystem system)
        {
            var apiApplication = system.AddContainer("API Application", "Provides Sepes functionality via a JSON/HTTPS API.", "ASP.NET Core");
            apiApplication.AddTags(Constants.DatabaseTag);
            return apiApplication;
        }

        public static Container CreateSepesDb(SoftwareSystem softwareSystem)
        {
            var db = softwareSystem.AddContainer("Sepes Database", "Stores studies, sandboxes, user data etc.", "Relational Database");
            db.AddTags(Constants.DatabaseTag);
            return db;
        }
    }
}
