using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace Sepes.RestApi.Model
{

    public class StudyDB
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public HashSet<PodDB> pods { get; set; }
        public HashSet<UserDB> sponsors { get; set; }
        public HashSet<UserDB> suppliers { get; set; }
        //public ImmutableHashSet<DataSetDB> datasets { get; set; }
        public bool archived { get; set; }

        // old model
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }


        public Study ToStudy()
        {
            var studyPods = pods != null ? pods.Select(p => p.ToPod()) : new HashSet<Pod>();
            var studySponsors = sponsors != null ? sponsors.Select(u => u.ToUser()) : new HashSet<User>();
            var studySuppliers = suppliers != null ? suppliers.Select(u => u.ToUser()) : new HashSet<User>();

            int[] studyUserIds = new int[]{};
            int[] studyDatasetIds = new int[]{};


            if (userIds != null) {
                studyUserIds = userIds;
            }
            if (datasetIds != null) {
                studyDatasetIds = datasetIds;
            }

            return new Study(studyName, studyId, studyPods, studySponsors, studySuppliers, new HashSet<DataSet>(), archived, studyUserIds, studyDatasetIds);
        }

    }
}
