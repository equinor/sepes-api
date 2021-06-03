using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class ResourceTagFactory
    {
        public static Dictionary<string, string> SandboxResourceTags(IConfiguration config, Study study, Sandbox sandbox)
        {
            var tags = CreateBaseTags(study.Name);

            DecorateWithManagedByTags(tags, config);
            DecorateWithCostAllocationTags(tags, config, study);
            DecorateWithStudyOwnerTags(tags, sandbox.Study);
            DecorateWithSandboxTags(tags, sandbox);

            return tags;
        }

        public static Dictionary<string, string> StudySpecificDatasourceResourceGroupTags(IConfiguration config, Study study)
        {
            var tags = CreateBaseTags(study.Name);

            DecorateWithManagedByTags(tags, config);
            DecorateWithCostAllocationTags(tags, config, study);
            DecorateWithStudyOwnerTags(tags, study);

            return tags;
        }

        public static Dictionary<string, string> StudySpecificDatasourceStorageAccountTags(IConfiguration config, Study study, string datasetName)
        {
            var tags = StudySpecificDatasourceResourceGroupTags(config, study);
            tags.Add("DatasetName", datasetName);
            return tags;
        }

        static Dictionary<string, string> CreateBaseTags(string studyName)
        {
            return new Dictionary<string, string>() { { "CreatedByMachine", Environment.MachineName }, { "StudyName", studyName } };
        }

        public static void DecorateWithManagedByTags(Dictionary<string, string> tags, IConfiguration config)
        {
            //Adds a "managed by"-tag to Azure resources. Sepes won't change resources that are missing this tag
            var managedByTagValue = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.MANAGED_BY);
            tags.Add(CloudResourceConstants.MANAGED_BY_TAG_NAME, managedByTagValue);
        }

        public static void DecorateWithCostAllocationTags(Dictionary<string, string> tags, IConfiguration config, Study study)
        {
            if (!String.IsNullOrWhiteSpace(study.WbsCode))
            {
                //Enables cost tracking of Azure resources. This should only be enabled in PROD with real WBS codes, because fake wbs codes used in DEV is causing pain for teams that track the costs.
                var costAllocationTypeTagName = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.COST_ALLOCATION_TYPE_TAG_NAME);
                tags.Add(costAllocationTypeTagName, "WBS");

                var costAllocationCodeTagName = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.COST_ALLOCATION_CODE_TAG_NAME);
                tags.Add(costAllocationCodeTagName, study.WbsCode);
            }          
        }

        public static void DecorateWithStudyOwnerTags(Dictionary<string, string> tags, Study study)
        {
            var ownerParticipant = study.StudyParticipants.FirstOrDefault(sp => sp.RoleName == StudyRoles.StudyOwner);

            // TODO: Get Owner Name and Email from Roles!
            if (ownerParticipant != null)
            {
                tags.Add("StudyOwnerName", ownerParticipant.User.FullName);
                tags.Add("StudyOwnerEmail", ownerParticipant.User.EmailAddress);
            }                   
        }

        public static void DecorateWithSandboxTags(Dictionary<string, string> tags, Sandbox sandbox)
        {            
            tags.Add("SandboxName", sandbox.Name);

            tags.Add("TechnicalContactName", sandbox.TechnicalContactName);
            tags.Add("TechnicalContactEmail", sandbox.TechnicalContactEmail);
        }
    }
}
