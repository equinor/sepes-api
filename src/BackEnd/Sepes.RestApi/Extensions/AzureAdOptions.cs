namespace Microsoft.AspNetCore.Authentication
{
    public class AzureAdOptions
    {
        public string CLIENT_ID { get; set; }
        public string CLIENT_SECRET { get; set; }
        public string INSTANCE { get; set; }
        public string DOMAIN { get; set; }
        public string TENANT_ID { get; set; }      
    }
}
