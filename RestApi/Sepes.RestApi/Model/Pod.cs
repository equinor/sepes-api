using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;

namespace Sepes.RestApi.Model
{
    //Internal version of pod model
    public class Pod
    {
        public ushort id { get; }
        public string name { get; }
        public int studyId { get; }
        public readonly bool allowAll;
        public readonly ImmutableList<Rule> incoming;
        public readonly ImmutableList<Rule> outgoing;
        public readonly ImmutableList<User> users;
        public readonly ImmutableList<DataSet> locked;
        public readonly ImmutableList<DataSet> loaded;

        public Pod(ushort id, string name, int studyId, bool allowAll, IEnumerable<Rule> incoming, IEnumerable<Rule> outgoing, 
                    IEnumerable<User> users, IEnumerable<DataSet> locked, IEnumerable<DataSet> loaded)
        {
            this.id = id;
            this.name = name;
            this.studyId = studyId;
            this.allowAll = allowAll;
            this.incoming = incoming.ToImmutableList();
            this.outgoing = outgoing.ToImmutableList();
            this.users = users.ToImmutableList();
            this.locked = locked.ToImmutableList();
            this.loaded = loaded.ToImmutableList();
        }

        public string networkName => $"{studyId}-{name.Replace(" ", "-")}-Network";
        public string subnetName => $"{studyId}-{name.Replace(" ", "-")}-SubNet";
        public string resourceGroupName => $"{studyId}-{name.Replace(" ", "-")}-ResourceGroup";
        public string networkSecurityGroupName => $"{studyId}-{name.Replace(" ", "-")}-NetworkSecurityGroup";
        public string addressSpace => $"10.{1 + id / 256}.{id % 256}.0/24";
    }
}