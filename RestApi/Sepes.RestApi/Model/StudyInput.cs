using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.Model
{

    public class StudyInput
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public HashSet<PodInput> pods { get; set; }
        public HashSet<string> sponsors { get; set; }
        public HashSet<string> suppliers { get; set; }
        public bool archived { get; set; }

        // old model
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }


        public Study ToStudy() // will be fully implemented later
        {
            return new Study(studyName, studyId, new List<Pod>(), new List<User>(), new List<User>(), new List<DataSet>(), archived, userIds, datasetIds);
        }
    }

}
