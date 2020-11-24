using Sepes.Infrastructure.Constants;
using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Constants
{
    public static class OperationsAndStudyRoles
    {
        static Dictionary<UserOperations, HashSet<string>> _operations;

        public static Dictionary<UserOperations, HashSet<string>> Operations { get { EnsureOperationExsits(); return _operations; } }

        static void EnsureOperationExsits()
        {

            if (_operations == null)
            {
                _operations = new Dictionary<UserOperations, HashSet<string>>() {

                    { UserOperations.Study_Create, new HashSet<string> {  } }, 
                    //Study overview and read
                    { UserOperations.Study_Read, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor, StudyRoles.SponsorRep } }, 
                    
                    //Study details and editation
                    { UserOperations.Study_Update_Metadata, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep } },
                    { UserOperations.Study_AddRemove_Dataset, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep } },
                    { UserOperations.Study_AddRemove_Participant, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep, StudyRoles.VendorAdmin } },
                    { UserOperations.Study_Crud_Sandbox, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep, StudyRoles.VendorAdmin } },
                    { UserOperations.Study_Close, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep } },
                    { UserOperations.Study_Delete, new HashSet<string> { StudyRoles.StudyOwner } },
                     
                    //Sandbox details and editation
                    { UserOperations.Study_Crud_Sandbox, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep, StudyRoles.VendorAdmin } },
                    { UserOperations.SandboxLock, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep, StudyRoles.VendorAdmin } },
                    { UserOperations.SandboxUnlock, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep, StudyRoles.VendorAdmin } }


                };
            }
        }

        public static HashSet<string> GetRequiredRoles(UserOperations operation)
        {
            return Operations[operation];
        }
    }

    public class OperationsAndRoles
    {
        Dictionary<Tuple<UserOperations, bool>, HashSet<string>> AllowedForApplicationRoles = new Dictionary<Tuple<UserOperations, bool>, HashSet<string>>();

        public OperationsAndRoles()
        {
            AddRole(UserOperations.Study_Create, false, AppRoles.Admin, AppRoles.Sponsor);
        }

        void AddRole(UserOperations operation, bool onlyForNonRestricted, params string[] allowedForRoles)
        {
            AllowedForApplicationRoles.Add(new Tuple<UserOperations, bool>(operation, onlyForNonRestricted), new HashSet<string>(allowedForRoles));
        }

        public HashSet<string> GetRequiredApplicationRoles(UserOperations operation, bool onlyForNonRestricted)
        {
            HashSet<string> roleSet = null;

            if(AllowedForApplicationRoles.TryGetValue(new Tuple<UserOperations, bool>(operation, onlyForNonRestricted), out roleSet))
            {
                return roleSet;
            }
            else 
            {
                return roleSet;
            }
        }

    }
}
