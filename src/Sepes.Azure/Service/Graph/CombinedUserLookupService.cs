using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Graph;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class CombinedUserLookupService : ICombinedUserLookupService
    {
        readonly ILogger _logger;
        readonly IConfiguration _configuration;
        readonly IUserFromGroupLookupService _employeeUserLookupService;
        readonly IUserFromGroupLookupService _affiliateUserLookupService;

        public CombinedUserLookupService(ILogger<CombinedUserLookupService> logger, IConfiguration configuration, IUserFromGroupLookupService employeeUserLookupService, IUserFromGroupLookupService affiliateUserLookupService)
        {
            _logger = logger;
            _configuration = configuration;
            _employeeUserLookupService = employeeUserLookupService;
            _affiliateUserLookupService = affiliateUserLookupService;
        }

        public async Task<Dictionary<string, AzureUserDto>> SearchAsync(string search, int limit, CancellationToken cancellationToken = default)
        {
            var lookupRequests = new List<IUserFromGroupRequest>()
            {
                CreateSearchTask("employee", _employeeUserLookupService, ConfigConstants.EMPLOYEE_GROUP_ID, search, limit),
                CreateSearchTask("affiliate", _affiliateUserLookupService, ConfigConstants.AFFILIATE_GROUP_ID, search, limit)
            };

            var lookupTasks = lookupRequests.Select(p => BufferCall(p, cancellationToken));          

            var completedLookupTasks = await Task.WhenAll(lookupTasks);

            return MergeRequestResults(completedLookupTasks);
        }

        IUserFromGroupRequest CreateSearchTask(string searchName, IUserFromGroupLookupService service, string groupConfigKey, string search, int limit)
        {
            var groupIdFromConfig = ConfigUtil.GetConfigValueAndThrowIfEmpty(_configuration, groupConfigKey);
            return new UserFromGroupRequest(searchName, service, groupIdFromConfig, search, limit);
        }

        private async Task<Dictionary<string, AzureUserDto>> BufferCall(IUserFromGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await request.StartRequest(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Graph search {request.SearchName} with search text {request.SearchText} failed", ex);
                return await Task.FromResult(new Dictionary<string, AzureUserDto>());
            }
        }      

        Dictionary<string, AzureUserDto> MergeRequestResults(Dictionary<string, AzureUserDto>[] results)
        {
            var mergedSearchResults = new Dictionary<string, AzureUserDto>();

            foreach (var curTask in results)
            {
                TryAddToDictionary(mergedSearchResults, curTask);
            }

            return mergedSearchResults;
        }

        void TryAddToDictionary(Dictionary<string, AzureUserDto> target, Dictionary<string, AzureUserDto> source)
        {
            foreach (var curKvp in source)
            {
                target.TryAdd(curKvp.Key, curKvp.Value);
            }
        }
    }
}
