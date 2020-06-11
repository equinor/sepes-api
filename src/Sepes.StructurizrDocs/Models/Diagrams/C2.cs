using Sepes.StructurizrDocs.Models.Containers;
using Sepes.StructurizrDocs.Models.SoftwareSystems.External;
using Structurizr;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

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
            var sepesSystem = SepesSystemFactory.Sepes(model);
            sepesUser.Uses(sepesSystem, "Uses");

            var singlePageApplication = sepesSystem.AddContainer("Single-Page Application", "Provides all of the Sepes functionality to users via their web browser.", "JavaScript and React");
            singlePageApplication.AddTags(Constants.WebBrowserTag);
       
            var apiApplication = sepesSystem.AddContainer("API Application", "Provides Sepes functionality via a JSON/HTTPS API.", "ASP.NET Core");

            var sepesDb = SepesContainerFactory.SepesDb(sepesSystem);

            sepesUser.Uses(singlePageApplication, "Uses", "");

            //EXTERNAL SYSTEMS
            var azureAd = AzureSystemFactory.AzureAD(model);
            sepesSystem.Uses(azureAd, "Authenticates");

            var azureIaaS = AzureSystemFactory.AzureIaaS(model);
            sepesSystem.Uses(azureIaaS, "Uses");


            //SETTING UP INTERNAL RELATIONSHIPS

            
            singlePageApplication.Uses(apiApplication, "Communicates with API", "HTTPS");
            apiApplication.Uses(sepesDb, "Reads from and writes to", "HTTPS");

            //SETTING UP EXTERNAL RELATIONSHIPS
            singlePageApplication.Uses(azureAd, "Authenticates User", "HTTPS");
            apiApplication.Uses(azureIaaS, "Uses", "HTTPS");           

            ContainerView containerView = views.CreateContainerView(sepesSystem, "Containers", "The container diagram for Sepes.");
            containerView.Add(sepesUser);
            containerView.AddAllContainers();
            containerView.Add(azureAd);
            containerView.Add(azureIaaS);        
            containerView.PaperSize = PaperSize.A5_Landscape;

            return workspace;


            //Container singlePageApplication = internetBankingSystem.AddContainer("Single-Page Application", "Provides all of the Internet banking functionality to customers via their web browser.", "JavaScript and Angular");
            //singlePageApplication.AddTags(WebBrowserTag);
            //Container mobileApp = internetBankingSystem.AddContainer("Mobile App", "Provides a limited subset of the Internet banking functionality to customers via their mobile device.", "Xamarin");
            //mobileApp.AddTags(MobileAppTag);
            //Container webApplication = internetBankingSystem.AddContainer("Web Application", "Delivers the static content and the Internet banking single page application.", "Java and Spring MVC");
            //Container apiApplication = internetBankingSystem.AddContainer("API Application", "Provides Internet banking functionality via a JSON/HTTPS API.", "Java and Spring MVC");
            //Container database = internetBankingSystem.AddContainer("Database", "Stores user registration information, hashed authentication credentials, access logs, etc.", "Relational Database Schema");
            //database.AddTags(DatabaseTag);

            //customer.Uses(webApplication, "Uses", "HTTPS");
            //customer.Uses(singlePageApplication, "Uses", "");
            //customer.Uses(mobileApp, "Uses", "");
            //webApplication.Uses(singlePageApplication, "Delivers to the customer's web browser", "");
            //apiApplication.Uses(database, "Reads from and writes to", "JDBC");
            //apiApplication.Uses(mainframeBankingSystem, "Uses", "XML/HTTPS");
            //apiApplication.Uses(emailSystem, "Sends e-mail using", "SMTP");



        }
    }
}
