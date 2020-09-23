namespace Sepes.Infrastructure.Constants
{
    public enum UserOperations
    {
        //Study overview
        StudyReadOwnRestricted,

        //Study details and editation
        StudyCreate,
        StudyUpdateMetadata,

        StudyAddRemoveDataset,
        StudyAddRemoveParticipant,
        StudyAddRemoveSandbox,

        StudyClose,
        StudyDelete,

        //Sandbox details and editation
        SandboxEdit,
        SandboxLock,
        SandboxUnlock,
        EditRules,
      
    }
}
