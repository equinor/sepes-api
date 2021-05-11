//using Sepes.Infrastructure.Model.SepesSqlModels;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Linq;

//namespace Sepes.Infrastructure.Dto
//{
//    //Internal version of pod model
//    public class PodDto
//    {
//        public ushort? id { get; }
//        public string name { get; }
//        public int studyId { get; }
//        public bool allowAll { get; } //Used to remove NSG from subnet
//        public ImmutableList<RuleDto> incoming { get; }
//        public ImmutableList<RuleDto> outgoing { get; }
//        public ImmutableList<DataSet> locked { get; }
//        public ImmutableList<DataSet> loaded { get; }

//        public string networkName => $"{studyId}-{name.Replace(" ", "-")}-Network";
//        public string subnetName => $"{studyId}-{name.Replace(" ", "-")}-SubNet";
//        public string resourceGroupName => $"{studyId}-{name.Replace(" ", "-")}-ResourceGroup";
//        public string networkSecurityGroupName => $"{studyId}-{name.Replace(" ", "-")}-NetworkSecurityGroup";
//        public string addressSpace => $"10.{1 + id / 256}.{id % 256}.0/24";


//        public PodDto(ushort? id, string name, int studyId, bool allowAll, IEnumerable<RuleDto> incoming, IEnumerable<RuleDto> outgoing, 
//                    IEnumerable<DataSet> locked, IEnumerable<DataSet> loaded)
//        {
//            this.id = id;
//            this.name = name;
//            this.studyId = studyId;
//            this.allowAll = allowAll;
//            this.incoming = incoming == null ? ImmutableList<RuleDto>.Empty : incoming.ToImmutableList();
//            this.outgoing = outgoing == null ? ImmutableList<RuleDto>.Empty : outgoing.ToImmutableList();
//            this.locked = locked == null ? ImmutableList<DataSet>.Empty : locked.ToImmutableList();
//            this.loaded = loaded == null ? ImmutableList<DataSet>.Empty : loaded.ToImmutableList();
//        }

//        public PodDto(ushort? id, string name, int studyId) : 
//            this(id, name, studyId, false, ImmutableList<RuleDto>.Empty, ImmutableList<RuleDto>.Empty, 
//            ImmutableList<DataSet>.Empty, ImmutableList<DataSet>.Empty) {}

//        public PodInputDto ToPodInput()
//        {
//            var inputIncoming = new List<RuleInputDto>();
//            var inputOutgoing = new List<RuleInputDto>();
//            var inputUsers = new List<string>();
            
//            foreach (RuleDto rule in incoming) {
//                inputIncoming.Add(rule.ToRuleInput());
//            }
//            foreach (RuleDto rule in outgoing) {
//                inputOutgoing.Add(rule.ToRuleInput());
//            }

//            return new PodInputDto(){
//                podId = id,
//                podName = name,
//                studyId = studyId,
//                openInternet = allowAll,
//                incoming = inputIncoming.ToArray(),
//                outgoing = inputOutgoing.ToArray()
//            };
//        }

//        public PodDB ToPodDB()
//        {
//            return new PodDB(){
//                id = (ushort) id,
//                name = name,
//                studyId = studyId,
//                allowAll = allowAll,
//                incoming = incoming.Select(rule => rule.ToRuleDB()).ToList(),
//                outgoing = outgoing.Select(rule => rule.ToRuleDB()).ToList()
//            };
//        }

//        public PodDto NewPodId(ushort newId)
//        {
//            return new PodDto(newId, name, studyId, allowAll, incoming, outgoing, locked, loaded);
//        }

//        public override bool Equals(object obj)
//        {
//            return obj is PodDto pod &&
//                   id == pod.id &&
//                   name == pod.name &&
//                   studyId == pod.studyId &&
//                   allowAll == pod.allowAll &&
//                   Enumerable.SequenceEqual(incoming, pod.incoming) &&
//                   Enumerable.SequenceEqual(outgoing, pod.outgoing);
//        }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(id, name);
//        }
//    }
//}
