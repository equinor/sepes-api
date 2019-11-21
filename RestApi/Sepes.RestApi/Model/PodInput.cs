using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.Model
{
    public class PodInput
    {
        public ushort podId { get; set; }
        public string podName { get; set; }
        public int studyId { get; set; }
        public string tag { get; set; }
        public RuleInput[] incoming { get; set; }
        public RuleInput[] outgoing { get; set; }
        public string[] users { get; set; }


        public Pod ToPod()
        {
            var podIncoming = incoming != null ? incoming.Select(r => r.ToRule()) : new List<Rule>();
            var podOutgoing = outgoing != null ? outgoing.Select(r => r.ToRule()) : new List<Rule>();
            var podUsers = users != null ? users.Select(u => new User("", u, "")) : new List<User>();

            return new Pod(podId, podName, studyId, false, podIncoming, podOutgoing, podUsers, new List<DataSet>(), new List<DataSet>());
        }
    }
}
