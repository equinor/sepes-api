using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceTagsFactory
    {
        public static string MANAGED_BY_TAG_NAME = "ManagedBy";

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
            tags.Add(MANAGED_BY_TAG_NAME, managedByTagValue);
        }

        public static void DecorateWithCostAllocationTags(Dictionary<string, string> tags, IConfiguration config, Study study)
        {
            //Enables cost tracking of Azure resources. This should only be enabled in PROD with real WBS codes, because fake wbs codes used in DEV is causing pain for teams that track the costs.
            var costAllocationTypeTagName = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.COST_ALLOCATION_TYPE_TAG_NAME);
            tags.Add(costAllocationTypeTagName, "WBS");

            var costAllocationCodeTagName = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.COST_ALLOCATION_CODE_TAG_NAME);
            tags.Add(costAllocationCodeTagName, study.WbsCode);
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

        public static Dictionary<string, string> CreateUnitTestTags(string studyName)
        {
            var tags = CreateBaseTags(studyName);
            tags.Add("IsUnitTest", "true");
            return tags;
            // var tags = new Dictionary<string, string>() { { "CreatedByMachine", Environment.MachineName } };
        }       

        public static string TagDictionaryToString(Dictionary<string, string> tags)
        {
            return JsonSerializer.Serialize(tags);
        }

        public static IDictionary<string, string> TagReadOnlyDictionaryToDictionary(IReadOnlyDictionary<string, string> tags)
        {
            return tags.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static Dictionary<string, string> TagStringToDictionary(string tags)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(tags);
        }

        public static void ContainsTagWithValueThrowIfError(IDictionary<string, string> resourceTags, string tagName, string expectedTagValue)
        {
            string actualTagValue;

            if (resourceTags.TryGetValue(tagName, out actualTagValue))
            {
                if (String.IsNullOrWhiteSpace(actualTagValue))
                {
                    throw new Exception($"Value of tag {tagName} was empty");
                }
                else
                {
                    if (expectedTagValue == actualTagValue)
                    {
                        return;
                    }
                    else
                    {
                        throw new Exception($"Value of tag {tagName} was different. Expected value: {expectedTagValue}, Actual value: {actualTagValue}");
                    }
                }
            }
            else
            {
                throw new Exception($"Resource is missing tag: {tagName}");
            }
        }

        public static void CheckIfResourceIsManagedByThisInstanceThrowIfNot(IConfiguration config, IDictionary<string, string> resourceTags)
        {
            var expectedTagValueFromConfig = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.MANAGED_BY);

            AzureResourceTagsFactory.ContainsTagWithValueThrowIfError(resourceTags, MANAGED_BY_TAG_NAME, expectedTagValueFromConfig);
        }

    }
}
