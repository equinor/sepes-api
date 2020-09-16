using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IPerformCloudResourceCRUD
    {
        Task<CloudResourceCRUDResult> Create(CloudResourceCRUDInput parameters);

        //Task<CloudResourceCRUDResult> Update(CloudResourceCRUDInput parameters);

        //Task<CloudResourceCRUDResult> Delete(CloudResourceCRUDInput parameters);
    }

    public class CloudResourceCRUDInput
    {
        public string ResourceGrupName { get; set; }
        public string Name { get; set; }

        public string SandboxName { get; set; }
        public string Region { get; set; }

        public Dictionary<string, string> Tags;

        public string CustomConfiguration { get; set; }
    }

    public class CloudResourceCRUDResult
    {
        public bool Success;
        public string CurrentProvisioningState { get; set; }

        public IResource Resource { get; set; }
    }
}
