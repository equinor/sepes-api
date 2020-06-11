using Structurizr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.StructurizrDocs.Models.Containers
{
    public static class SepesContainerFactory
    {
        public static Container SepesDb(SoftwareSystem softwareSystem)
        {
            var db = softwareSystem.AddContainer("Database", "Stores studies, sandboxes, user data etc.", "Relational Database Schema");
            db.AddTags(Constants.DatabaseTag);
            return db;
        }
    }
}
