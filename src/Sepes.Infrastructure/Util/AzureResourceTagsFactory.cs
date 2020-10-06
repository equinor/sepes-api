using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto;
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

        public static Dictionary<string, string> CreateTags(IConfiguration config, string studyName, StudyDto study, SandboxDto sandbox)
        {
            var tags = CreateBaseTags(studyName);

            //Adds a "managed by"-tag to Azure resources. Sepes won't change resources that are missing this tag
            var managedByTagValue = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.MANAGED_BY);
            tags.Add(MANAGED_BY_TAG_NAME, managedByTagValue);

            //Enables cost tracking of Azure resources. This should only be enabled in PROD with real WBS codes, because fake wbs codes used in DEV is causing pain for teams that track the costs.
            var costAllocationTypeTagName = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.COST_ALLOCATION_TYPE_TAG_NAME);
            tags.Add(costAllocationTypeTagName, "WBS");

            var costAllocationCodeTagName = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.COST_ALLOCATION_CODE_TAG_NAME);
            tags.Add(costAllocationCodeTagName, study.WbsCode);

            // TODO: Get Owner Name and Email from Roles!
            //tags.Add("StudyOwnerName", study.OwnerName);
            //tags.Add("StudyOwnerEmail", study.OwnerEmail);
            tags.Add("SandboxName", sandbox.Name);
            tags.Add("TechnicalContactName", sandbox.TechnicalContactName);
            tags.Add("TechnicalContactEmail", sandbox.TechnicalContactEmail);

            return tags;
        }

        public static Dictionary<string, string> CreateUnitTestTags(string studyName)
        {
            var tags = CreateBaseTags(studyName);
            tags.Add("IsUnitTest", "true");
            return tags;
            // var tags = new Dictionary<string, string>() { { "CreatedByMachine", Environment.MachineName } };
        }

        static Dictionary<string, string> CreateBaseTags(string studyName)
        {
            return new Dictionary<string, string>() { { "CreatedByMachine", Environment.MachineName }, { "StudyName", studyName } };
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

        public static bool ContainsTagWithValue(IDictionary<string, string> resourceTags, string tagName, string expectedTagValue)
        {
            string actualTagValue;

            if (resourceTags.TryGetValue(tagName, out actualTagValue))
            {
                if (!String.IsNullOrWhiteSpace(actualTagValue))
                {
                    if (expectedTagValue == actualTagValue)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool ResourceIsManagedByThisInstance(IConfiguration config, IDictionary<string, string> resourceTags)
        {
            var expectedTagValueFromConfig = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.MANAGED_BY);

            if (AzureResourceTagsFactory.ContainsTagWithValue(resourceTags, MANAGED_BY_TAG_NAME, expectedTagValueFromConfig))
            {
                return true;
            }

            return false;

        }

    }
}
