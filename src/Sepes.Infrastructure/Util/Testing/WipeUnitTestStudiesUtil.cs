//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Sepes.Infrastructure.Util.Testing
//{
//    public static class WipeUnitTestStudiesUtil
//    {
//        public async Task NukeUnitTestSandboxes()
//        {
//            var deleteTasks = new List<Task>();

//            //Get list of resource groups
//            var resourceGroups = await _resourceGroupService.GetResourceGroupsAsList();
//            foreach (var resourceGroup in resourceGroups)
//            {
//                //If resource group has unit-test prefix, nuke it
//                if (resourceGroup.Name.Contains(SandboxResourceProvisioningService.UnitTestPrefix))
//                {
//                    // TODO: Mark as deleted in SEPES DB
//                    deleteTasks.Add(_resourceGroupService.Delete(resourceGroup.Name));
//                }
//            }

//            await Task.WhenAll(deleteTasks);
//            return;
//        }
//    }
//}
