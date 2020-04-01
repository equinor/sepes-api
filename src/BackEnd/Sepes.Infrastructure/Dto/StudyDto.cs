using Sepes.Infrastructure.Model.SepesSqlModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace Sepes.Infrastructure.Dto
{
    public class StudyDto
    {
        public string studyName { get; }
        public int studyId { get; }
        public ImmutableHashSet<PodDto> pods { get; }
        public ImmutableHashSet<UserDto> sponsors { get; }
        public ImmutableHashSet<UserDto> suppliers { get; }
        public ImmutableHashSet<DataSet> datasets { get; }
        public bool archived { get; }


        public StudyDto(string studyName, int studyId, IEnumerable<PodDto> pods, IEnumerable<UserDto> sponsors, 
                    IEnumerable<UserDto> suppliers, IEnumerable<DataSet> datasets, bool archived)
        {
            this.studyName = studyName;
            this.studyId = studyId;
            this.pods = pods == null ? ImmutableHashSet<PodDto>.Empty : pods.ToImmutableHashSet();
            this.sponsors = sponsors == null ? ImmutableHashSet<UserDto>.Empty : sponsors.ToImmutableHashSet();
            this.suppliers = suppliers == null ? ImmutableHashSet<UserDto>.Empty : suppliers.ToImmutableHashSet();
            this.datasets = datasets == null ? ImmutableHashSet<DataSet>.Empty : datasets.ToImmutableHashSet();
            this.archived = archived;
        }

        public StudyDto(string studyName, int studyId, IEnumerable<PodDto> pods = null) : 
            this(studyName, studyId, pods == null ? ImmutableHashSet<PodDto>.Empty : pods, ImmutableHashSet<UserDto>.Empty, 
                ImmutableHashSet<UserDto>.Empty, ImmutableHashSet<DataSet>.Empty, false) {}

        public StudyInputDto ToStudyInput()
        {
            var inputPods = new HashSet<PodInputDto>();
            var inputSponsors = new HashSet<string>();
            var inputSuppliers = new HashSet<string>();

            foreach (PodDto pod in pods) {
                inputPods.Add(pod.ToPodInput());
            }
            foreach (UserDto user in sponsors) {
                inputSponsors.Add(user.userEmail);
            }
            foreach (UserDto user in suppliers) {
                inputSuppliers.Add(user.userEmail);
            }

            return new StudyInputDto(){
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

        public StudyDto ReplacePods(IEnumerable<PodDto> newPods)
        {
            return new StudyDto(studyName, studyId, newPods, sponsors, suppliers, datasets, archived);
        }
        
        public override bool Equals(object obj)
        {
            return obj is StudyDto study &&
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
