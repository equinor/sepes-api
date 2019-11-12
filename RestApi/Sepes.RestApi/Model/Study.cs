using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.Model
{

    public class Study
    {
        public string studyName { get; set; }
        public int studyId { get; set; }
        public List<Pod> pods { get; set; }
        public List<string> owners { get; set; }
        public List<string> users { get; set; }
        public bool archived { get; set; }

        // old model
        public int[] userIds { get; set; }
        public int[] datasetIds { get; set; }


        public override bool Equals(object obj)
        {
            return obj is Study study &&
                   studyName == study.studyName &&
                   studyId == study.studyId &&
                   Enumerable.SequenceEqual(userIds, study.userIds) &&
                   Enumerable.SequenceEqual(datasetIds, study.datasetIds) &&
                   Enumerable.SequenceEqual(pods, study.pods) &&
                   Enumerable.SequenceEqual(owners, study.owners) &&
                   Enumerable.SequenceEqual(users, study.users) &&
                   archived == study.archived;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(studyName, studyId, pods, owners, users, archived, userIds, datasetIds);
        }
    }
}
