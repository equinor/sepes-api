using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace Sepes.RestApi.Model
{

    public class Study
    {
        public string studyName { get; }
        public int studyId { get; }
        public ImmutableHashSet<Pod> pods { get; }
        public ImmutableHashSet<User> sponsors { get; }
        public ImmutableHashSet<User> suppliers { get; }
        public ImmutableHashSet<DataSet> datasets { get; }
        public bool archived { get; }

        // old model
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }

        public Study(string studyName, int studyId, IEnumerable<Pod> pods, IEnumerable<User> sponsors, 
                    IEnumerable<User> suppliers, IEnumerable<DataSet> datasets, bool archived, int[] userIds, int[] datasetIds)
        {
            this.studyName = studyName;
            this.studyId = studyId;
            this.pods = pods.ToImmutableHashSet();
            this.sponsors = sponsors.ToImmutableHashSet();
            this.suppliers = suppliers.ToImmutableHashSet();
            this.datasets = datasets.ToImmutableHashSet();
            this.archived = archived;
            this.userIds = userIds;
            this.datasetIds = datasetIds;
        }

        public override bool Equals(object obj)
        {
            return obj is Study study &&
                   studyName == study.studyName &&
                   studyId == study.studyId &&
                   Enumerable.SequenceEqual(pods, study.pods) &&
                   Enumerable.SequenceEqual(sponsors, study.sponsors) &&
                   Enumerable.SequenceEqual(suppliers, study.suppliers) &&
                   Enumerable.SequenceEqual(datasets, study.datasets) &&
                   archived == study.archived &&
                   Enumerable.SequenceEqual(userIds, study.userIds) &&
                   Enumerable.SequenceEqual(datasetIds, study.datasetIds);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(studyName, studyId, pods, sponsors, suppliers, archived, userIds, datasetIds);
        }

        public StudyInput ToStudyInput() // will be fully implemented later
        {
            return new StudyInput(){
                studyId = studyId,
                studyName = studyName,
                archived = archived
            };
        }
    }
}
