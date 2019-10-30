namespace Sepes.RestApi.Model
{

    public class NsgModel
    {
        public string securityGroupName { get; set; }
        //public string securityGroupNameOld {get; set;} Sadly not usable as of now.
        public string resourceGroupName { get; set; }
        public string subnetName { get; set; }
        public string networkName { get; set; }
        public int priority { get; set; }
        public string[] externalAddresses { get; set; }
        public string[] internalAddresses { get; set; }
    }

}