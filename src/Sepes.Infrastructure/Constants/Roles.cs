using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Constants
{
    public static class Roles
    {
        public const string Admin = "Sepes-Admin";
        public const string Sponsor = "Sepes-Sponsor";       
    }

    public static class RoleSets
    {
        public const string AdminOrSponsor = Roles.Admin + "," + Roles.Sponsor;    
    }

  

}
