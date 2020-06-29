using Sepes.StructurizrDocs.Models.SoftwareSystems.External;
using Structurizr;
using Structurizr.IO.C4PlantUML.ModelExtensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Sepes.StructurizrDocs.Models.Diagrams
{
    public static class C1
    {
       

        public static Workspace Create()
        {
            var workspace = new Workspace("Sepes C2", "Sepes C1 Context Diagram.");

            var model = workspace.Model;

            model.Enterprise = new Enterprise("Equinor");

            var user = model.AddPerson("User", "A user of my software system.");
            var softwareSystem = model.AddSoftwareSystem("SEPES", "SEPES Application.");

            var azureIaaS = model.AddSoftwareSystem(Location.External, "Azure IaaS", "VMs, Networking, Storage");
            var azureAppi = model.AddSoftwareSystem(Location.External, "Azure Application Insights", "Logging and usage statistics");


            user.Uses(softwareSystem, "Uses");

            softwareSystem.Uses(azureIaaS, "Uses");
            softwareSystem.Uses(azureAppi, "Uses");

            ViewSet viewSet = workspace.Views;
            SystemContextView contextView = viewSet.CreateSystemContextView(softwareSystem, "SystemContext", "An example of a System Context diagram.");
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            contextView.Relationships
            .First(rv => rv.Relationship.SourceId == user.Id && rv.Relationship.DestinationId == softwareSystem.Id)
            .SetDirection(DirectionValues.Right);

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });

            return workspace;
        }
    }
}
