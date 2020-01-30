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


        public Study(string studyName, int studyId, IEnumerable<Pod> pods, IEnumerable<User> sponsors, 
                    IEnumerable<User> suppliers, IEnumerable<DataSet> datasets, bool archived)
        {
            this.studyName = studyName;
            this.studyId = studyId;
            this.pods = pods == null ? ImmutableHashSet<Pod>.Empty : pods.ToImmutableHashSet();
            this.sponsors = sponsors == null ? ImmutableHashSet<User>.Empty : sponsors.ToImmutableHashSet();
            this.suppliers = suppliers == null ? ImmutableHashSet<User>.Empty : suppliers.ToImmutableHashSet();
            this.datasets = datasets == null ? ImmutableHashSet<DataSet>.Empty : datasets.ToImmutableHashSet();
            this.archived = archived;
        }

        public Study(string studyName, int studyId, IEnumerable<Pod> pods = null) : 
            this(studyName, studyId, pods == null ? ImmutableHashSet<Pod>.Empty : pods, ImmutableHashSet<User>.Empty, 
                ImmutableHashSet<User>.Empty, ImmutableHashSet<DataSet>.Empty, false) {}

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
                suppliers = inputSuppliers
            };
        }

        public StudyDB ToStudyDB()
        {
            return new StudyDB(){
                studyId = studyId,
                studyName = studyName,
                archived = archived,
                pods = pods.Select(pod => pod.ToPodDB()).ToHashSet(),
                sponsors = sponsors.Select(user => user.ToUserDB()).ToHashSet(),
                suppliers = suppliers.Select(user => user.ToUserDB()).ToHashSet(),
                datasets = datasets.Select(dataset => dataset.ToDataSetDB()).ToHashSet()
            };
        }

        public Study ReplacePods(IEnumerable<Pod> newPods)
        {
            return new Study(studyName, studyId, newPods, sponsors, suppliers, datasets, archived);
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
                   archived == study.archived;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(studyName, studyId);
        }

    }
}
