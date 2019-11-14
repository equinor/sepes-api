using System.Collections.Generic;

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
            var podIncoming = new List<Rule>();
            var podOutgoing = new List<Rule>();
            var podUsers = new List<User>();
            
            foreach (RuleInput rule in incoming) {
                podIncoming.Add(new Rule(rule.port, rule.ip));
            }
            foreach (RuleInput rule in outgoing) {
                podOutgoing.Add(new Rule(rule.port, rule.ip));
            }
            foreach (string user in users) {
                podUsers.Add(new User("", user, ""));
            }

            return new Pod(podId, podName, studyId, false, podIncoming, podOutgoing, podUsers, new List<DataSet>(), new List<DataSet>());
        }
    }
}
