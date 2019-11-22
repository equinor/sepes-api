using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;

namespace Sepes.RestApi.Model
{
    //Internal version of pod model
    public class PodDB
    {
        public ushort id { get; set; }
        public string name { get; set; }
        public int studyId { get; set; }
        public bool allowAll { get; set; }
        public List<RuleDB> incoming { get; set; }
        public List<RuleDB> outgoing { get; set; }
        //public List<User> users { get; set; }
        //public ImmutableList<DataSet> locked2 { get; set; }
        //public ImmutableList<DataSet> loaded2 { get; set; }


        public Pod ToPod()
        {
            var podIncoming = incoming != null ? incoming.Select(r => r.ToRule()) : new List<Rule>();
            var podOutgoing = outgoing != null ? outgoing.Select(r => r.ToRule()) : new List<Rule>();
            var podUsers = /*users != null ? users :*/ new List<User>();

            return new Pod(id, name, studyId, false, podIncoming, podOutgoing, podUsers, new List<DataSet>(), new List<DataSet>());
        }
        
    }
}
