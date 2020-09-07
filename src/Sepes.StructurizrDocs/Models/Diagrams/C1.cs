using Structurizr;
using Structurizr.IO.C4PlantUML.ModelExtensions;
using System.Linq;

namespace Sepes.StructurizrDocs.Models.Diagrams
{
    public static class C1
    {

        public static Workspace CreateWorkspaceWithCommonModel()
        {           
            var sepesWorkspace = SepesModel.CreateSepesModel(1);

            var sepesSystem = sepesWorkspace.Model.SoftwareSystems.FirstOrDefault();
            
            var sepesViews = sepesWorkspace.Views;           

            SystemContextView contextView = sepesViews.CreateSystemContextView(sepesSystem, "SystemContext", "Sepes System Context Diagram.");
            contextView.AddAllPeople();            
            contextView.AddAllSoftwareSystems();                

            contextView.PaperSize = PaperSize.A5_Landscape;
            contextView.EnterpriseBoundaryVisible = false;
            DefaultStyleDecorator.Decorate(sepesViews);

         

            return sepesWorkspace;
        }
   
    }
}
