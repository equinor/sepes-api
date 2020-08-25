using Structurizr;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class SepesSystemFactory
    {
        public static SoftwareSystem CreateAndAddSepesSystemToModel(Model model)
        {
            var sepesSystem = model.AddSoftwareSystem(Location.Internal, "Sepes", "Allows users to create and maintain Studies, Sandboxes, Datasets and more.");          

            return sepesSystem;
        }

        public static SoftwareSystem CreateAndAddAzureSystemToModel(Model model)
        {
            var azureSystem = model.AddSoftwareSystem(Location.External, "Azure", "Provides cloud computing services.");

            return azureSystem;
        }
    }
}
