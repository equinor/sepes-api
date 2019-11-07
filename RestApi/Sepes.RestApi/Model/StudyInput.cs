namespace Sepes.RestApi.Model
{

    public class StudyInput
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }
    }

}
