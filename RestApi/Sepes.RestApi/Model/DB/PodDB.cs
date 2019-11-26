using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.Model
{
    public class PodDB
    {
        public ushort id { get; set; }
        public string name { get; set; }
        public int studyId { get; set; }
        public bool allowAll { get; set; }
        public List<RuleDB> incoming { get; set; }
        public List<RuleDB> outgoing { get; set; }
        public List<UserDB> users { get; set; }
        //public ImmutableList<DataSetDB> locked { get; set; }
        //public ImmutableList<DataSetDB> loaded { get; set; }


        public Pod ToPod()
        {
            var podIncoming = incoming != null ? incoming.Select(r => r.ToRule()) : new List<Rule>();
            var podOutgoing = outgoing != null ? outgoing.Select(r => r.ToRule()) : new List<Rule>();
            var podUsers = users != null ? users.Select(u => u.ToUser()) : new List<User>();

            return new Pod(id, name, studyId, false, podIncoming, podOutgoing, podUsers, new List<DataSet>(), new List<DataSet>());
        }
        
    }
}
