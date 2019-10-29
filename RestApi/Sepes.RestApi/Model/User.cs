namespace Sepes.RestApi.Model
{

    public class User
    {
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string userGroup { get; set; }

        
        // The name to show in UI.
        public readonly string DisplayName;
        // Just a value given from Opa. That should not be changes but given back when needed.
        // This may become a different type than string.
        // Do not send to client.
        public readonly string OpaReference;
        // Reference to the user as its understood by the azure api for when we need to give resource access.
        // This may become a different type than string.
        // Do not send to client.
        public readonly string AzureReference;
        //Best to seperate into two models like Pod has been. So sensitive information is not sent out
    }

}
