namespace Sepes.RestApi.Model
{

    public class Study
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }
        public bool archived { get; set; }
    }

}
