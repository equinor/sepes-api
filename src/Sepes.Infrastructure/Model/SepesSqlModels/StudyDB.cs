//using Sepes.Infrastructure.Dto;
//using System.Collections.Generic;
//using System.Linq;


//namespace Sepes.Infrastructure.Model.SepesSqlModels
//{

//    public class StudyDB
//    {
//        public string studyName { get; set; }
//        public int studyId { get; set; }
//        public HashSet<PodDB> pods { get; set; }
//        public HashSet<UserDB> sponsors { get; set; }
//        public HashSet<UserDB> suppliers { get; set; }
//        public HashSet<DataSetDB> datasets { get; set; }
//        public bool archived { get; set; }


//        public StudyDto ToStudy()
//        {
//            var studyPods = pods != null ? pods.Select(p => p.ToPod()) : new HashSet<PodDto>();
//            var studySponsors = sponsors != null ? sponsors.Select(u => u.ToUser()) : new HashSet<UserDto>();
//            var studySuppliers = suppliers != null ? suppliers.Select(u => u.ToUser()) : new HashSet<UserDto>();

//            return new StudyDto(studyName, studyId, studyPods, studySponsors, studySuppliers, new HashSet<DataSet>(), archived);
//        }

//    }
//}
