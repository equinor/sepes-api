using System.Collections.Generic;

namespace Sepes.RestApi.Model
{

    public class StudyInput
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public List<Pod> pods { get; set; }
        public List<string> owners { get; set; }
        public List<string> users { get; set; }

        // old model
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }
    }

}
