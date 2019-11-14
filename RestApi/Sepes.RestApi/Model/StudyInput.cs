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


        public Study ToStudy()
        {
            var studyPods = new HashSet<Pod>();
            var studySponsors = new HashSet<User>();
            var studySuppliers = new HashSet<User>();

            foreach (PodInput pod in pods) {
                studyPods.Add(pod.ToPod());
            }
            foreach (string user in sponsors) {
                studySponsors.Add(new User("", user, ""));
            }
            foreach (string user in suppliers) {
                studySuppliers.Add(new User("", user, ""));
            }

            return new Study(studyName, studyId, studyPods, studySponsors, studySuppliers, new HashSet<DataSet>(), archived, userIds, datasetIds);
        }
    }
}
