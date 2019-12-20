using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.Model
{
    public class PodInput
    {
        public ushort? podId { get; set; }
        public string podName { get; set; }
        public int studyId { get; set; }
        public string tag { get; set; }
        public RuleInput[] incoming { get; set; } = new RuleInput[]{};
        public RuleInput[] outgoing { get; set; } = new RuleInput[]{};
        public string[] users { get; set; } = new string[]{};
        //Corresponds to allowAll internally
        public bool openInternet { get; set; }


        public Pod ToPod()
        {
            var podIncoming = incoming.Select(r => r.ToRule());
            var podOutgoing = outgoing.Select(r => r.ToRule());
            var podUsers = users.Select(u => new User("", u, ""));

            return new Pod(podId, podName, studyId, openInternet, podIncoming, podOutgoing, podUsers, new List<DataSet>(), new List<DataSet>());
        }
    }
}
