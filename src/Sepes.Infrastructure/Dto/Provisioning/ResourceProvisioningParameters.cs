using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Provisioning
{
    public class ResourceProvisioningParameters
    {
        
            public int DatabaseId { get; set; }
            public string ResourceGroupName { get; set; }
            public string Name { get; set; }

            public string StudyName { get; set; }

            public int SandboxId { get; set; }
            public string SandboxName { get; set; }
            public Region Region { get; set; }

            public Dictionary<string, string> Tags;

            public string NetworkSecurityGroupName { get; set; }

            public string ConfigurationString { get; set; }


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
                ResourceGroupName = null;
                Name = null;
                SandboxId = 0;
                SandboxName = null;
                Region = null;
                ConfigurationString = null;

                if (Tags != null)
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
}
