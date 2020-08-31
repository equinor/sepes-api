namespace Sepes.Infrastructure.Constants
{
    public static class Roles
    {
        //AD Roles
        public const string Admin = "Sepes-Admin";
        public const string Sponsor = "Sepes-Sponsor";
        public const string DatasetAdmin = "Sepes-Dataset-Admin";

        //Study specific roles, maintained in Sepes application
        public const string SponsorRep = "Sponsor Rep";
        public const string VendorAdmin= "Vendor Admin";
        public const string VendorContributor = "Vendor Contributor";
        public const string StudyViewer = "Study Viewer";
        public const string StudySponsor = "Sponsor";
    }

    public static class RoleSets
    {
        public const string AdminOrSponsor = Roles.Admin + "," + Roles.Sponsor;    
    }

  

}
