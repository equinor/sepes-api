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

        public Pod(ushort id, string name, int studyId)
        {
            this.id = id;
            this.name = name;
            this.studyId = studyId;
        }

        public string networkName => $"{studyId}-{name.Replace(" ", "-")}-Network";
        public string subnetName => $"{studyId}-{name.Replace(" ", "-")}-SubNet";
        public string resourceGroupName => $"{studyId}-{name.Replace(" ", "-")}-ResourceGroup";
        public string networkSecurityGroupName => $"{studyId}-{name.Replace(" ", "-")}-NetworkSecurityGroup";
        public string addressSpace => $"10.{1 + id / 256}.{id % 256}.0/24";

        // if true ignore port rules and just allow all incoming and outgoing traffick.
        public readonly bool allowAll;
        // The ushort is the port. And the uint is the IPv4 address. We will limit us to IPv4 for the beta.
        // This is the list of ports from the outside that is allowed to talk to the servers inside the pod.
        // But only from certant ip addresses.
        public readonly ImmutableDictionary<ushort, ImmutableList<IPAddress>> inGoing;
        // Same as InComming but now its what ports and ip addresses on the internet servers in a pod can talk to.
        public readonly ImmutableDictionary<ushort, ImmutableList<IPAddress>> outGoing;
        // The users that have access to changes the azure resources for this pod.
        // Will most likely com from the study list of users.
        public readonly ImmutableList<User> users;
        // List of datasets that have their rules applied to this pod.
        // Servers in this pod do not have access to the data at this point.
        // and DataSets may be removed.
        public readonly ImmutableList<DataSet> locked;
        // List of datasets loaded in this pods.
        // Just as with loaded the rules apply.
        // You can't remove a dataset from the loaded list.
        public readonly ImmutableList<DataSet> loaded;
    }
}