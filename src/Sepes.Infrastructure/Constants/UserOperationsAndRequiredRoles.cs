using System.Collections.Generic;

namespace Sepes.Infrastructure.Constants
{
    public static class UserOperationsAndRequiredRoles
    {
        //Kommer inn med Liste over StudyParticipant
        //Lurer på om Role-teksten er i en av 
        //Her bygger vi datasettet over operasjoner 
        static Dictionary<UserOperations, HashSet<string>> _operations;
        public static Dictionary<UserOperations, HashSet<string>> Operations { get { EnsureOperationExsits(); return _operations; } }

        static void EnsureOperationExsits()
        {

            if (_operations == null)
            {
                _operations = new Dictionary<UserOperations, HashSet<string>>() {

                    { UserOperations.StudyCreate, new HashSet<string> {  } }, 
                    //Study overview and read
                    { UserOperations.StudyRead, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor, StudyRoles.SponsorRep } }, 
                    
                    //Study details and editation
                    { UserOperations.StudyUpdateMetadata, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep } },
                    { UserOperations.StudyAddRemoveDataset, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep } },
                    { UserOperations.StudyAddRemoveParticipant, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep, StudyRoles.VendorAdmin } },
                    { UserOperations.StudyAddRemoveSandbox, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep, StudyRoles.VendorAdmin } },
                    { UserOperations.StudyClose, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep } },
                    { UserOperations.StudyDelete, new HashSet<string> { StudyRoles.StudyOwner } },
                     
                    //Sandbox details and editation
                    { UserOperations.SandboxEdit, new HashSet<string> { StudyRoles.StudyOwner, StudyRoles.SponsorRep, StudyRoles.VendorAdmin } },
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
}
