using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureCredentialService
    {
        AzureCredentials GetAzureCredentials();
    }
}
