using Structurizr;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class SepesSystemFactory
    {
        public static SoftwareSystem Create(Model model)
        {
            var sepesSystem = model.AddSoftwareSystem(Location.Internal, "Sepes", "Allows users to administer Studies and subcomponents.");          

            return sepesSystem;
        }
    }
}
