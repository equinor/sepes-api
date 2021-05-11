namespace Sepes.Common.Constants
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
        Sandbox_IncreasePhase,
        Sandbox_Unlock,
        Sandbox_EditInboundRules,
        Sandbox_OpenInternet,

        //Dataset
        PreApprovedDataset_Read,
        PreApprovedDataset_Create_Update_Delete
      
    }
}
