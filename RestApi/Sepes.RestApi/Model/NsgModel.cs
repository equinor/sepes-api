namespace Sepes.RestApi.Model
{

    public class NsgModel
    {
        public string securityGroupName { get; set; }
        public string securityGroupNameOld {get; set;}
        public string resourceGroupName { get; set; }
        public string subnetName { get; set; }
        public string networkId { get; set; }
        public int priority { get; set; }
        public string[] externalAddresses { get; set; }
        public string[] internalAddresses { get; set; }
    }

}