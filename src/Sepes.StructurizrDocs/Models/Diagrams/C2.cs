using Structurizr;
using System.Linq;

namespace Sepes.StructurizrDocs.Models.Diagrams
{
    public static class C2
    {

        public static Workspace CreateWorkspaceWithCommonModel()
        {          
            var sepesWorkspace = SepesModel.CreateSepesModel(2);

            var sepesSystem = sepesWorkspace.Model.SoftwareSystems.FirstOrDefault();
            var sepesViews = sepesWorkspace.Views;
            var sepesContainerView = sepesViews.CreateContainerView(sepesSystem, "Sepes Containers", "Sepes Container diagram.");
                        
         
            sepesContainerView.AddAllContainers();
            sepesContainerView.AddAllPeople();
            sepesContainerView.AddAllSoftwareSystems();
  
            sepesContainerView.PaperSize = PaperSize.A5_Landscape;
            
            DefaultStyleDecorator.Decorate(sepesViews);

            return sepesWorkspace;
        }
    }
}
