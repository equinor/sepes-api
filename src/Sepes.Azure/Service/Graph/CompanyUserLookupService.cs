using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class CompanyUserLookupService : UserFromGroupLookupService, ICompanyUserLookupService
    {
        readonly string _groupId;
        readonly ILogger _logger;
        readonly IConfiguration _configuration;

        public CompanyUserLookupService(IGraphServiceProvider graphServiceProvider, ILogger<CompanyUserLookupService> logger, IConfiguration configuration)
            : base("GroupMember.Read.All", graphServiceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _groupId = ConfigUtil.GetConfigValueAndThrowIfEmpty(_configuration, ConfigConstants.EMPLOYEE_GROUP_ID);
        }

        public async Task<Dictionary<string, AzureUserDto>> SearchAsync(string search, int limit, CancellationToken cancellationToken = default)
        {
            try
            {
                return await SearchInternalAsync(_groupId, search, limit, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Company user search failed for search text {search}", ex);
            }

            return new Dictionary<string, AzureUserDto>();
        }
    }
}
