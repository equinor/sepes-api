using Sepes.StructurizrDocs.Models.Diagrams;
using Structurizr;
using Structurizr.IO.C4PlantUML;
using System;
using System.IO;

namespace Sepes.StructurizrDocs
{
    class Program
    {
        const string C1_PATH = @"D:\Workspace\SepesPlantUml\Sepes_C1_PlantUML.puml";
        const string C2_PATH = @"D:\Workspace\SepesPlantUml\Sepes_C2_PlantUML.puml";
        const string C2SF_PATH = @"D:\Workspace\SepesPlantUml\SepesSF_C2_PlantUML.puml";

        static void Main(string[] args)
        {
            //CreateC1();
            //CreateC2();
            CreateC2SF();
        }

        static void CreateC1()
        {
            var c1 = C1.CreateWorkspaceWithCommonModel();
            ExportDiagram(c1, C1_PATH);

        }

        static void CreateC2()
        {
            var c2 = C2.CreateWorkspaceWithCommonModel();
            ExportDiagram(c2, C2_PATH);

        }

        static void CreateC2SF()
        {
            var workspace = new Workspace("SepesC2", "Sepes C2 Diagram.");
            var model = workspace.Model;

            var user = model.AddPerson(Location.Internal, "User", "Equinor Employee/Consultant or external supplier");

            //DEFINE SEPES SYSTEM WITH CONTAINERS

            var sepesSystem = model.AddSoftwareSystem(Location.Internal, "Sepes", "Allows users to create and maintain Studies, Sandboxes, Datasets and more.");

            var singlePageApplication = sepesSystem.AddContainer("Single-Page Application", "Provides all of the Sepes functionality to users via their web browser.", "React");
            var apiApplication = sepesSystem.AddContainer("API", "Provides Sepes functionality via a JSON/HTTPS API.", "ASP.NET Core");
            var azureFunction = sepesSystem.AddContainer("Azure Functions", "Creates and monitors cloud resources in Azure.", ".NET Core");
            var db = sepesSystem.AddContainer("Database", "Stores studies, sandboxes, user data etc.", "Azure SQL Server");

            //DEFINE AZURE CONTAINERS        
         
            var azureAd = model.AddSoftwareSystem(Location.External, "Azure Active Directory", "Provides authentication and authorization");
            var azureIaaS = model.AddSoftwareSystem(Location.External, "Azure Iaas", "Hosts Sandbox resources like Network, VMs, Storage and more.Also used for queue, diagnostics storage and more");
            var azureApplicationInsights = model.AddSoftwareSystem(Location.External, "Azure Application Insights", "Logging and usage statistics"); 

            //SET UP SEPES CONTAINER RELATIONSHIPS
            user.Uses(singlePageApplication, "Uses", "");
            singlePageApplication.Uses(apiApplication, "Communicates with", "HTTPS");
            apiApplication.Uses(azureFunction, "Communicates with", "HTTPS");
            apiApplication.Uses(db, "Reads/writes", "HTTPS");
            azureFunction.Uses(db, "Reads/writes", "HTTPS");

            //SET UP EXTERNAL RELATONSHIPS         
            singlePageApplication.Uses(azureAd, "Authenticates user with"); 
            singlePageApplication.Uses(azureApplicationInsights, "Sends logs/usage to");

            apiApplication.Uses(azureApplicationInsights, "Sends logs/usage to");

            azureFunction.Uses(azureIaaS, "Creates and Maintains");
            azureFunction.Uses(azureApplicationInsights, "Sends logs/usage to");          

            var sepesContainerView = workspace.Views.CreateContainerView(sepesSystem, "Sepes Containers", "Shows all Sepes containers.");
        
            sepesContainerView.AddAllContainers();
            sepesContainerView.AddAllPeople();
            sepesContainerView.AddAllSoftwareSystems();
            sepesContainerView.PaperSize = PaperSize.A5_Landscape;        

            ExportDiagram(workspace, C2SF_PATH);
        }

        static void ExportDiagram(Workspace workspace, string filePath)
        {
            using (var stringWriter = new StringWriter())
            {
                var plantUmlWriter = new C4PlantUmlWriter();
                plantUmlWriter.Write(workspace, stringWriter);
                var plantUmlString = stringWriter.ToString();
                File.WriteAllText(filePath, plantUmlString);
                Console.WriteLine("Exporting diagram to: " + filePath);
                Console.WriteLine(plantUmlString);
            }
        }



    }
}
