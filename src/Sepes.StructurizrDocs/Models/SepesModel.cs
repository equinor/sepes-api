using Sepes.StructurizrDocs.Models.Containers;
using Sepes.StructurizrDocs.Models.SoftwareSystems.External;
using Structurizr;

namespace Sepes.StructurizrDocs.Models
{
    public class SepesModel
    {
        public static Workspace CreateSepesModel(int level)
        {
            var sepesWorkspace = new Workspace("Sepes", "Sepes Application Diagram.");
            var sepesModel = sepesWorkspace.Model;

            sepesModel.Enterprise = new Enterprise("Equinor");          

            //INTERNAL SYSTEMS
            var sepesSystem = SepesSystemFactory.CreateAndAddSepesSystemToModel(sepesModel);
            var sepesUser = SepesUserFactory.AddUser(sepesModel, sepesSystem);          

            var singlePageApplication = SepesContainerFactory.CreateAndAddSepesFrontEnd(sepesSystem);
            var apiApplication = SepesContainerFactory.CreateAndAddSepesApi(sepesSystem);
            var workerApplication = SepesContainerFactory.CreateAndAddSepesWorker(sepesSystem);
            var sepesDb = SepesContainerFactory.CreateAndAddSepesDb(sepesSystem);

            sepesUser.Uses(singlePageApplication, "Uses", "");

            singlePageApplication.Uses(apiApplication, "Communicates with", "HTTPS");
            apiApplication.Uses(workerApplication, "Communicates with", "HTTPS");

            TheseUsesSepesDb(sepesDb, apiApplication, workerApplication);

            //EXTERNAL SYSTEMS

            if (level == 1)
            {         
                AzureSystemFactory.AddAzureAsABlackBox(sepesModel, sepesSystem);
            }
            else
            {
                AddExternalSystemsForLevel2AndDown(sepesModel, sepesSystem, singlePageApplication, apiApplication, workerApplication);
            }

            return sepesWorkspace;
        }
        

        static void AddExternalSystemsForLevel2AndDown(Model model, SoftwareSystem system, Container singlePageApplication, Container apiApplication, Container workerApplication)
        {
            var azureAd = AzureSystemFactory.AddAzureAd(model, system);

            var azureResourcesForSandboxes = AzureSystemFactory.AddAzureIaaS(model, system);

            var azureAppi = AzureSystemFactory.AddAzureAppInsight(model, system);

            //RELATIONSHIPS
            singlePageApplication.Uses(azureAd, "Authenticates user with", "HTTPS");
            TheseMaintainsAzureResourcesForSandbox(workerApplication, azureResourcesForSandboxes);
            TheseUsesApplicationInsights(azureAppi, singlePageApplication, apiApplication, workerApplication);
        }

        static void TheseUsesSepesDb(Container db, params Container[] containers)
        {
            foreach (var curContainer in containers)
            {
                curContainer.Uses(db, "Reads/writes", "HTTPS");
            }
        }

        static void TheseUsesApplicationInsights(SoftwareSystem appi, params Container[] containers)
        {
            foreach (var curContainer in containers)
            {
                curContainer.Uses(appi, "Sends logs/usage to", "HTTPS");
            }
        }

        static void TheseMaintainsAzureResourcesForSandbox(Container container, params SoftwareSystem[] azureServices)
        {
            foreach (var curAzureService in azureServices)
            {
                container.Uses(curAzureService, "Uses", "HTTPS");
            }
        }
    }
}
