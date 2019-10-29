namespace Sepes.RestApi.Model
{
    // Read only view of the current or proposed state of a Pod.
    // Missing technical parameters like Id.
    public class DataSet
    {
        // Name to show user.
        public readonly string DisplayName;
        // Link to the policy file or document.
        public readonly string OpaPolicy;
        // Reference to where the data is stored in azure.
        public readonly string AzureReference;
    }
}
