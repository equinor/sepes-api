using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Model.SepesSqlModels
{
    public class PodDB
    {
        public ushort id { get; set; }
        public string name { get; set; }
        public int studyId { get; set; }
        public bool allowAll { get; set; }
        public List<RuleDB> incoming { get; set; }
        public List<RuleDB> outgoing { get; set; }
        //public ImmutableList<DataSetDB> locked { get; set; }
        //public ImmutableList<DataSetDB> loaded { get; set; }


        public PodDto ToPod()
        {
            var podIncoming = incoming != null ? incoming.Select(r => r.ToRule()) : new List<RuleDto>();
            var podOutgoing = outgoing != null ? outgoing.Select(r => r.ToRule()) : new List<RuleDto>();

            return new PodDto(id, name, studyId, allowAll, podIncoming, podOutgoing, new List<DataSet>(), new List<DataSet>());
        }
        
    }
}
