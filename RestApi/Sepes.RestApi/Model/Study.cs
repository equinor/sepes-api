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

        public StudyInput ToStudyInput()
        {
            var inputPods = new HashSet<PodInput>();
            var inputSponsors = new HashSet<string>();
            var inputSuppliers = new HashSet<string>();

            foreach (Pod pod in pods) {
                inputPods.Add(pod.ToPodInput());
            }
            foreach (User user in sponsors) {
                inputSponsors.Add(user.userEmail);
            }
            foreach (User user in suppliers) {
                inputSuppliers.Add(user.userEmail);
            }

            return new StudyInput(){
                studyId = studyId,
                studyName = studyName,
                archived = archived,
                pods = inputPods,
                sponsors = inputSponsors,
                suppliers = inputSuppliers,
                userIds = userIds,
                datasetIds = datasetIds
            };
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

    }
}
