using System.Collections.Generic;
using System.Collections.Immutable;

namespace Sepes.RestApi.Model
{

    public class Study
    {
        public readonly string studyName;
        public readonly int studyId;
        public readonly ImmutableHashSet<Pod> pods;
        public readonly ImmutableHashSet<User> sponsors;
        public readonly ImmutableHashSet<User> suppliers;
        public readonly ImmutableHashSet<DataSet> datasets;
        public readonly bool archived;

        // old model
        public readonly int[] userIds;
        public readonly int[] datasetIds;

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

    }
}
