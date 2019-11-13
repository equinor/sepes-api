using System;

namespace Sepes.RestApi.Model
{
    // Read only view of the current or proposed state of a Pod.
    // Missing technical parameters like Id.
    public class DataSet
    {
        // Name to show user.
        public readonly string displayName;
        // Link to the policy file or document.
        public readonly string opaPolicy;
        // Reference to where the data is stored in azure.
        public readonly string azureReference;

        public DataSet(string name, string opaPolicy, string azureReference)
        {
            this.displayName = name;
            this.opaPolicy = opaPolicy;
            this.azureReference = azureReference;
        }

        public override bool Equals(object obj)
        {
            return obj is DataSet dataset &&
                   displayName == dataset.displayName &&
                   opaPolicy == dataset.opaPolicy &&
                   azureReference == dataset.azureReference;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(displayName, opaPolicy, azureReference);
        }
    }
}
