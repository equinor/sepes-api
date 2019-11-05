namespace Sepes.RestApi.Model
{

    public class Study
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }
        public bool archived { get; set; }

        public Study(string studyName, int studyId, int[] userIds, int[] datasetIds, bool archived)
        {
            this.studyName = studyName;
            this.studyId = studyId;
            this.userIds = userIds;
            this.datasetIds = datasetIds;
            this.archived = archived;
        }
    }
}
