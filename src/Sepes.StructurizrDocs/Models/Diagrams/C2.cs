using Sepes.StructurizrDocs.Models.Containers;
using Sepes.StructurizrDocs.Models.SoftwareSystems.External;
using Structurizr;

namespace Sepes.StructurizrDocs.Models.Diagrams
{
    public static class C2
    {       

        public static Workspace Create()
        {
            var workspace = new Workspace("Sepes", "C2 Container Diagram for Sepes Application.");
            var model = workspace.Model;
            var views = workspace.Views;

            model.Enterprise = new Enterprise("Equinor");

            var sepesUser = SepesUserFactory.SepesUser(model);


            //INTERNAL SYSTEMS
            var sepesSystem = SepesSystemFactory.Create(model);
            sepesUser.Uses(sepesSystem, "Uses");

            var singlePageApplication = SepesContainerFactory.CreateSepesFrontEnd(sepesSystem);
            var apiApplication = SepesContainerFactory.CreateSepesApi(sepesSystem);

            var sepesDb = SepesContainerFactory.CreateSepesDb(sepesSystem);

            sepesUser.Uses(singlePageApplication, "Uses", "");

            //EXTERNAL SYSTEMS
            var azureAd = AzureSystemFactory.AzureAd(model);
            sepesSystem.Uses(azureAd, "Authenticates");

            var azureIaaS = AzureSystemFactory.AzureIaaS(model);
            sepesSystem.Uses(azureIaaS, "Uses");

            var azureAppi = AzureSystemFactory.AzureAppInsight(model);
            sepesSystem.Uses(azureAppi, "Uses");            

            //SETTING UP INTERNAL RELATIONSHIPS            
            singlePageApplication.Uses(apiApplication, "Communicates with API", "HTTPS");
            apiApplication.Uses(sepesDb, "Reads from and writes to", "HTTPS");
          

            //SETTING UP EXTERNAL RELATIONSHIPS
            singlePageApplication.Uses(azureAd, "Authenticates", "HTTPS");
            apiApplication.Uses(azureIaaS, "Uses", "HTTPS");
            apiApplication.Uses(azureAppi, "Writes logs and usage statistics", "HTTPS");

            //Set up container view

            ContainerView containerView = views.CreateContainerView(sepesSystem, "Containers", "The container diagram for Sepes.");
            containerView.Add(sepesUser);
            containerView.AddAllContainers();
            containerView.Add(azureAd);
            containerView.Add(azureIaaS);
            containerView.Add(azureAppi);
            containerView.PaperSize = PaperSize.A5_Landscape;

            DefaultStyleDecorator.Decorate(views);

            return workspace;  
        }
    }
}
