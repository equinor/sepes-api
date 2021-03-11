using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class SandboxSeed
    {
        public static async Task<Sandbox> Create(
                Study study,
                string sandboxName = SandboxConstants.NAME,
                string region = TestConstants.REGION,
                SandboxPhase phase = SandboxPhase.Open,
             bool addDatasets = false)
        {
            var sandbox = SandboxBasic(study.Id, study.Name, sandboxName, region, phase);

            AddDatasets(addDatasets, study, sandbox);

            return await SliceFixture.InsertAsync(sandbox);
        }

        static Sandbox SandboxBasic(int studyId, string studyName, string sandboxName, string region, SandboxPhase phase = SandboxPhase.Open)
        {
            var sb = new Sandbox()
            {
                StudyId = studyId,
                Name = sandboxName,
                Region = region,
                PhaseHistory = new List<SandboxPhaseHistory>(),
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow,
                TechnicalContactEmail = "seedcreator@somesystem.com",
                TechnicalContactName = "Seed",
                Resources = SandboxResources(region, studyName, sandboxName)

            };

            int counter = 0;

            for (var curPhase = SandboxPhase.Open; curPhase <= phase; curPhase++)
            {
                sb.PhaseHistory.Add(new SandboxPhaseHistory() { Counter = counter, Phase = curPhase, CreatedBy = "seed" });
                counter++;
            }

            return sb;
        }

        static List<CloudResource> SandboxResources(string region, string studyName, string sandboxName)
        {
            var resourceGroupName = AzureResourceNameUtil.SandboxResourceGroup(studyName, sandboxName);
            var resourceGroup = CloudResourceFactory.CreateResourceGroup(region, resourceGroupName, purpose: CloudResourcePurpose.SandboxResourceGroup, sandboxControlled: true);

            var result = new List<CloudResource>() { resourceGroup };
            result.Add(CloudResourceFactory.Create(region, AzureResourceType.StorageAccount, resourceGroupName, AzureResourceNameUtil.DiagnosticsStorageAccount(studyName, sandboxName), parentResource: resourceGroup, sandboxControlled: true));
            result.Add(CloudResourceFactory.Create(region, AzureResourceType.NetworkSecurityGroup, resourceGroupName, AzureResourceNameUtil.NetworkSecGroup(studyName, sandboxName), parentResource: resourceGroup, sandboxControlled: true));
            result.Add(CloudResourceFactory.Create(region, AzureResourceType.VirtualNetwork, resourceGroupName, AzureResourceNameUtil.VNet(studyName, sandboxName), parentResource: resourceGroup, sandboxControlled: true));
            result.Add(CloudResourceFactory.Create(region, AzureResourceType.Bastion, resourceGroupName, AzureResourceNameUtil.Bastion(studyName, sandboxName), parentResource: resourceGroup, sandboxControlled: true));

            return result;
        }

        static void AddDatasets(bool addDatasets, Study study, Sandbox sandbox)
        {
            if (addDatasets)
            {
                if (study.StudyDatasets != null)
                {
                    sandbox.SandboxDatasets = new List<SandboxDataset>();

                    foreach (var curDs in study.StudyDatasets)
                    {
                        sandbox.SandboxDatasets.Add(new SandboxDataset() { DatasetId = curDs.DatasetId, Sandbox = sandbox, Added = DateTime.UtcNow, AddedBy = "seed"  });
                    }
                }
            }

        }

    }
}
