using Structurizr;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class SepesUserFactory
    {

        public static Person User(Model model)
        {
            var sepesUser = model.AddPerson(Location.Internal, "User", "Equinor Employee/Consultant or external supplier");
            sepesUser.AddTags(Constants.SepesUserTag);
            sepesUser.AddTags(Tags.Person);
            return sepesUser;
        }

        public static Person AddUser(Model model, SoftwareSystem system)
        {
            var sepesUser = User(model);
            sepesUser.Uses(system, "Uses");
            return sepesUser;
        }


        public static Person InternalUser(Model model)
        {
            var sepesUser = model.AddPerson(Location.Internal, "Internal User", "Equinor Employee/Consultant etc");
            sepesUser.AddTags(Constants.SepesUserTag);
            sepesUser.AddTags(Tags.Person);
            return sepesUser;
        }

        public static Person ExternalUser(Model model)
        {
            var sepesUser = model.AddPerson(Location.External, "External User", "Works for supplier");
            sepesUser.AddTags(Constants.SepesUserTag);
            sepesUser.AddTags(Tags.Person);
            return sepesUser;
        }
    }
}
