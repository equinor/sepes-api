using System.Collections.Generic;
using System.Collections.Immutable;

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
        public int[] userIds { get; }
        public int[] datasetIds { get; }

        public Study(string studyName, int studyId, IEnumerable<Pod> pods, IEnumerable<User> sponsors, 
                    IEnumerable<User> suppliers, IEnumerable<DataSet> datasets, bool archived, int[] userIds, int[] datasetIds)
        {
            this.studyName = studyName;
            this.studyId = studyId;
            this.pods = pods == null ? ImmutableHashSet<Pod>.Empty : pods.ToImmutableHashSet();
            this.sponsors = sponsors == null ? ImmutableHashSet<User>.Empty : sponsors.ToImmutableHashSet();
            this.suppliers = suppliers == null ? ImmutableHashSet<User>.Empty : suppliers.ToImmutableHashSet();
            this.datasets = datasets == null ? ImmutableHashSet<DataSet>.Empty : datasets.ToImmutableHashSet();
            this.archived = archived;
            this.userIds = userIds;
            this.datasetIds = datasetIds;
        }

        public Study(string studyName, int studyId, IEnumerable<Pod> pods = null) : 
            this(studyName, studyId, pods == null ? ImmutableHashSet<Pod>.Empty : pods, ImmutableHashSet<User>.Empty, 
                ImmutableHashSet<User>.Empty, ImmutableHashSet<DataSet>.Empty, false, new int[]{}, new int[]{}) {}

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
