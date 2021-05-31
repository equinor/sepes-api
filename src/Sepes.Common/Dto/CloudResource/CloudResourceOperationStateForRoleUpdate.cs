namespace Sepes.Common.Dto
{
    public class CloudResourceOperationStateForRoleUpdate
    {
        public CloudResourceOperationStateForRoleUpdate()
        {
          
        }

        public CloudResourceOperationStateForRoleUpdate(int studyId)
        {
            StudyId = studyId;   
        }

        public int StudyId { get; set; }    
    }
}
