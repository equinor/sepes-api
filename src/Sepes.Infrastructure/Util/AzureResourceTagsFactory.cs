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

        public static Dictionary<string, string> CreateTags(string mangedByTagValue, string studyName, StudyDto study, SandboxDto sandbox)
        {
            var tags = CreateBaseTags(studyName);
            tags.Add(MANAGED_BY_TAG_NAME, mangedByTagValue);
            tags.Add("WBS", study.WbsCode);
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
