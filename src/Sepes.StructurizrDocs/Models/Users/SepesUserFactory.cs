using Structurizr;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class SepesUserFactory
    {
        public static Person SepesUser(Model model)
        {
            var sepesUser = model.AddPerson(Location.External, "Sepes User", "A user of SEPES, either an Equinor Cmployee or External.");
            sepesUser.AddTags(Constants.SepesUserTag);

            return sepesUser;
        }
    }
}
