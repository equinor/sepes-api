using Structurizr;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class SepesUserFactory
    {
        public static Person SepesUser(Model model)
        {
            var sepesUser = model.AddPerson(Location.External, "Sepes User", "Equinor Employee or External.");
            sepesUser.AddTags(Constants.SepesUserTag);
            sepesUser.AddTags(Tags.Person);
            return sepesUser;
        }    
    }
}
