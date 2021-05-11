//using System.Collections.Generic;
//using System.Linq;

//namespace Sepes.Infrastructure.Dto
//{

//    public class StudyInputDto
//    {
//        public string studyName { get; set; }
//        public int studyId { get; set; }
//        public HashSet<PodInputDto> pods { get; set; } = new HashSet<PodInputDto>();
//        public HashSet<string> sponsors { get; set; } = new HashSet<string>();
//        public HashSet<string> suppliers { get; set; } = new HashSet<string>();
//        public bool archived { get; set; }


//        public StudyDto_OLD ToStudy()
//        {
//            var studyPods = new HashSet<PodDto>();
//            var studySponsors = new HashSet<UserDto>();
//            var studySuppliers = new HashSet<UserDto>();

//            foreach (PodInputDto pod in pods) {
//                studyPods.Add(pod.ToPod());
//            }
//            foreach (string user in sponsors) {
//                studySponsors.Add(new UserDto("", user, ""));
//            }
//            foreach (string user in suppliers) {
//                studySuppliers.Add(new UserDto("", user, ""));
//            }

//            return new StudyDto_OLD(studyName, studyId, studyPods, studySponsors, studySuppliers, new HashSet<DataSet>(), archived);
//        }
//    }
//}
