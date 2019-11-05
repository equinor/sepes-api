namespace Sepes.RestApi.Model
{

    public class StudyInput
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }

        public StudyInput(string studyName, int studyId, int[] userIds, int[] datasetIds)
        {
            this.studyName = studyName;
            this.studyId = studyId;
            this.userIds = userIds;
            this.datasetIds = datasetIds;
        }
    }

}
