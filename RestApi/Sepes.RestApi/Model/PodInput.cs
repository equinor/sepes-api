namespace Sepes.RestApi.Model
{

    public class PodInput
    {
        public int podID { get; set; }
        public string podName { get; set; }
        public int studyId { get; set; }
        public string tag { get; set; }


        public PodInput(string podName, int studyId)
        {
            this.podName =podName;
            this.studyId =studyId;
        }
    }

}
