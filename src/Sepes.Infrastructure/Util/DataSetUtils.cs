using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class DatasetUtils
    {

        public static void UpdateDatasetBasicDetails(Dataset datasetFromDb, DatasetCreateUpdateInputBaseDto updatedDataset)
        {

            if (!String.IsNullOrWhiteSpace(updatedDataset.Name) && updatedDataset.Name != datasetFromDb.Name)
            {
                datasetFromDb.Name = updatedDataset.Name;
            }

            if (!String.IsNullOrWhiteSpace(updatedDataset.Classification) && updatedDataset.Classification != datasetFromDb.Classification)
            {
                datasetFromDb.Classification = updatedDataset.Classification;
            }

            if (updatedDataset.DataId != 0 && updatedDataset.DataId != datasetFromDb.DataId)
            {
                datasetFromDb.DataId = updatedDataset.DataId;
            }
        }

        public static void PerformUsualTestForPostedDatasets(DatasetCreateUpdateInputBaseDto datasetDto)
        {
            if (String.IsNullOrWhiteSpace(datasetDto.Name))
            {
                throw new ArgumentException($"Field Dataset.Name is required. Current value: {datasetDto.Name}");
            }
            if (String.IsNullOrWhiteSpace(datasetDto.Classification))
            {
                throw new ArgumentException($"Field Dataset.Classification is required. Current value: {datasetDto.Classification}");
            }
            if (String.IsNullOrWhiteSpace(datasetDto.Location))
            {
                throw new ArgumentException($"Field Dataset.Location is required. Current value: {datasetDto.Location}");
            }           
        }

        public static List<FirewallRule> TranslateAllowedIpsToGenericFirewallRule(List<DatasetFirewallRule> datasetFirewallRules)
        {
            return datasetFirewallRules.Select(r => new FirewallRule() { Action = FirewallRuleAction.Allow, Address = r.Address }).ToList();
        }

        public static string TranslateAllowedIpsToOperationDesiredState(List<DatasetFirewallRule> datasetFirewallRules)
        {
            if (datasetFirewallRules.Count == 0)
            {
                return null;
            }

            var translated = TranslateAllowedIpsToGenericFirewallRule(datasetFirewallRules);

            return CloudResourceConfigStringSerializer.Serialize(translated);
        }

        public static async Task SetDatasetFirewallRules(UserDto user, Dataset dataset, string clientIp)
        {
            //add state
            dataset.FirewallRules = new List<DatasetFirewallRule>();

            //Add user's client IP

            if (clientIp != "::1")
            {
                dataset.FirewallRules.Add(CreateClientRule(user, clientIp));
            }

            //Add application IP, so that it can upload/download files 

            dataset.FirewallRules.Add(await CreateServerRuleAsync(user));
        }

        static DatasetFirewallRule CreateClientRule(UserDto user, string clientIp)
        {
            return CreateRule(user, DatasetFirewallRuleType.Client, clientIp);
        }

        public async static Task<DatasetFirewallRule> CreateServerRuleAsync(UserDto user)
        {
            var serverPublicIp = await IpAddressUtil.GetServerPublicIp();
            return CreateRule(user, DatasetFirewallRuleType.Api, serverPublicIp);
        }

        static DatasetFirewallRule CreateRule(UserDto user, DatasetFirewallRuleType ruleType, string ipAddress)
        {
            return new DatasetFirewallRule() { CreatedBy = user.UserName, Created = DateTime.UtcNow, RuleType = ruleType, Address = ipAddress };
        }

       public static CloudResource GetStudySpecificStorageAccountResourceEntry(Dataset dataset)
        {
            if(dataset.StudyId.HasValue && dataset.StudyId.Value > 0)
            {
                if(dataset.Resources == null)
                {
                    return null;
                }
                else
                {
                    return dataset.Resources.SingleOrDefault(r => r.ResourceType == AzureResourceType.StorageAccount && r.Purpose == CloudResourcePurpose.StudySpecificDatasetStorageAccount);
                }
             
            }

            throw new ArgumentException("Only supports Study Specific Dataset");
        }

        public static 

    }
}
