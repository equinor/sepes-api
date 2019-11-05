namespace Sepes.RestApi.Model
{

    public class PodInput
    {
        public int id { get; set; }
        public string name { get; set; }
        public int studyId { get; set; }
        public string tag { get; set; }


        public PodInput(string name, int studyId)
        {
            this.name =name;
            this.studyId =studyId;
        }
    }

}
