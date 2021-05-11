using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureStorageAccountCorsRuleService : AzureStorageAccountBaseService, IAzureStorageAccountCorsRuleService
    {
        public AzureStorageAccountCorsRuleService(IConfiguration config, ILogger<AzureStorageAccountCorsRuleService> logger)
            : base(config, logger)
        {
          
        }
        
        public async Task SetCorsRules(string resourceGroupName, string resourceName, List<Dto.CorsRule> rules, CancellationToken cancellationToken = default)
        {
            var corsRulesProperties = new List<Microsoft.Azure.Management.Storage.Fluent.Models.CorsRule>();

            foreach (var curRuleInput in rules)
            {
                corsRulesProperties.Add(new Microsoft.Azure.Management.Storage.Fluent.Models.CorsRule()
                {
                    AllowedOrigins = new List<string> { curRuleInput.Address },
                    AllowedHeaders = new List<string>() { "*" },
                    AllowedMethods = new List<AllowedMethods> { AllowedMethods.GET, AllowedMethods.POST, AllowedMethods.PUT, AllowedMethods.MERGE, AllowedMethods.DELETE  },                
                    MaxAgeInSeconds = 0,
                    ExposedHeaders = new List<string>(),
                });
            }

            var blobServiceProperties = new BlobServicePropertiesInner(cors: new CorsRules(corsRulesProperties));
            await _azure.StorageAccounts.Manager.BlobServices.Inner.SetServicePropertiesAsync(resourceGroupName, resourceName, blobServiceProperties, cancellationToken);                     
        
        }
    }
}
