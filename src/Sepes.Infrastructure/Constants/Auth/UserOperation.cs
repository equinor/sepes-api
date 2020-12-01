namespace Sepes.Infrastructure.Constants
{
    public enum UserOperation
    {
        //STUDY
        Study_Create,
        Study_Read,
        Study_Close,
        Study_Delete,
        Study_Update_Metadata,
        Study_Read_ResultsAndLearnings,
        Study_Update_ResultsAndLearnings,

        Study_AddRemove_Participant,

        Study_AddRemove_Dataset,

        //Sandbox
        Study_Crud_Sandbox,

        //Sandbox details and editation
        SandboxLock,
        SandboxUnlock,
        EditRules,

        //Dataset
        PreApprovedDataset_Read,
        PreApprovedDataset_Create_Update_Delete
      
    }
}
