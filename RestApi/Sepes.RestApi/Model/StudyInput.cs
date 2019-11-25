using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.Model
{

    public class StudyInput
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public HashSet<PodInput> pods { get; set; } = new HashSet<PodInput>();
        public HashSet<string> sponsors { get; set; } = new HashSet<string>();
        public HashSet<string> suppliers { get; set; } = new HashSet<string>();
        public bool archived { get; set; }

        // old model
        public int[] userIds { get; set; } = new int[]{};
        public int[] datasetIds { get; set; } = new int[]{};


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
