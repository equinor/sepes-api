using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IPerformCloudResourceCRUD
    {
        Task<CloudResourceCRUDResult> Create(CloudResourceCRUDInput parameters);

        //Task<CloudResourceCRUDResult> Update(CloudResourceCRUDInput parameters);

        Task<CloudResourceCRUDResult> Delete(CloudResourceCRUDInput parameters);
    }

    public class CloudResourceCRUDInput
    {
        public string ResourceGrupName { get; set; }
        public string Name { get; set; }

        public string StudyName { get; set; }
        public string SandboxName { get; set; }
        public Region Region { get; set; }

        public Dictionary<string, string> Tags;

        public string CustomConfiguration { get; set; }

        Dictionary<string, string> _sharedVariables = new Dictionary<string, string>();

        public bool TryGetSharedVariable(string key, out string value)
        {
            if (_sharedVariables.TryGetValue(key, out value))
            {
                return true;
            }

            value = null;
            return false;
        }

        void ClearProperties()
        {
            ResourceGrupName = null;
            Name = null;
            SandboxName = null;
            Region = null; 
            CustomConfiguration = null;

            if(Tags != null)
            {
                Tags.Clear();
            }
        }

        public void ResetButKeepSharedVariables(Dictionary<string, string> appendTheseVariables = null)
        {
            ClearProperties();

            if (appendTheseVariables != null && appendTheseVariables.Count > 0)
            {
                foreach (var curAppend in appendTheseVariables)
                {
                    _sharedVariables[curAppend.Key] = curAppend.Value;
                }
            }
        }

        public void ResetAndClearSharedVariables()
        {
            ClearProperties();
            _sharedVariables.Clear();
        }
    }

    public class CloudResourceCRUDResult
    {
        public bool Success;
        public string CurrentProvisioningState { get; set; }

        public IResource Resource { get; set; }

        public Microsoft.Rest.Azure.IResource NetworkResource { get; set; }

        public Dictionary<string, string> NewSharedVariables { get; set; } = new Dictionary<string, string>();
    }
}
