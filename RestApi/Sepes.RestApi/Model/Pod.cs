using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;

namespace Sepes.RestApi.Model
{
    //Internal version of pod model
    public class Pod
    {
        public ushort? id { get; }
        public string name { get; }
        public int studyId { get; }
        public bool allowAll { get; } //Used to remove NSG from subnet
        public ImmutableList<Rule> incoming { get; }
        public ImmutableList<Rule> outgoing { get; }
        public ImmutableList<DataSet> locked { get; }
        public ImmutableList<DataSet> loaded { get; }

        public string networkName => $"{studyId}-{name.Replace(" ", "-")}-Network";
        public string subnetName => $"{studyId}-{name.Replace(" ", "-")}-SubNet";
        public string resourceGroupName => $"{studyId}-{name.Replace(" ", "-")}-ResourceGroup";
        public string networkSecurityGroupName => $"{studyId}-{name.Replace(" ", "-")}-NetworkSecurityGroup";
        public string addressSpace => $"10.{1 + id / 256}.{id % 256}.0/24";


        public Pod(ushort? id, string name, int studyId, bool allowAll, IEnumerable<Rule> incoming, IEnumerable<Rule> outgoing, 
                    IEnumerable<DataSet> locked, IEnumerable<DataSet> loaded)
        {
            this.id = id;
            this.name = name;
            this.studyId = studyId;
            this.allowAll = allowAll;
            this.incoming = incoming == null ? ImmutableList<Rule>.Empty : incoming.ToImmutableList();
            this.outgoing = outgoing == null ? ImmutableList<Rule>.Empty : outgoing.ToImmutableList();
            this.locked = locked == null ? ImmutableList<DataSet>.Empty : locked.ToImmutableList();
            this.loaded = loaded == null ? ImmutableList<DataSet>.Empty : loaded.ToImmutableList();
        }

        public Pod(ushort? id, string name, int studyId) : 
            this(id, name, studyId, false, ImmutableList<Rule>.Empty, ImmutableList<Rule>.Empty, 
            ImmutableList<DataSet>.Empty, ImmutableList<DataSet>.Empty) {}

        public PodInput ToPodInput()
        {
            var inputIncoming = new List<RuleInput>();
            var inputOutgoing = new List<RuleInput>();
            var inputUsers = new List<string>();
            
            foreach (Rule rule in incoming) {
                inputIncoming.Add(rule.ToRuleInput());
            }
            foreach (Rule rule in outgoing) {
                inputOutgoing.Add(rule.ToRuleInput());
            }

            return new PodInput(){
                podId = id,
                podName = name,
                studyId = studyId,
                openInternet = allowAll,
                incoming = inputIncoming.ToArray(),
                outgoing = inputOutgoing.ToArray()
            };
        }

        public Pod NewPodId(ushort newId)
        {
            return new Pod(newId, name, studyId, allowAll, incoming, outgoing, locked, loaded);
        }

        public override bool Equals(object obj)
        {
            return obj is Pod pod &&
                   id == pod.id &&
                   name == pod.name &&
                   studyId == pod.studyId &&
                   allowAll == pod.allowAll &&
                   Enumerable.SequenceEqual(incoming, pod.incoming) &&
                   Enumerable.SequenceEqual(outgoing, pod.outgoing);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, name);
        }
    }
}