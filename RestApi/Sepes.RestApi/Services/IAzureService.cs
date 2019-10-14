using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sepes.RestApi.Model;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace Sepes.RestApi.Services
{
    public interface IAzureService
    {
        IResourceGroup CreateResourceGroup(int podID, string podName, string podTag);
        void TerminateResourceGroup(string resourceGroupName);
    }
}
