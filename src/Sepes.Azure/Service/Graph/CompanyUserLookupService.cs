using Microsoft.Extensions.Configuration;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class CompanyUserLookupService : UserFromGroupLookupService, ICompanyUserLookupService
    {
        readonly string _groupId;
        readonly IConfiguration _configuration;

        public CompanyUserLookupService(IConfiguration configuration, IGraphServiceProvider graphServiceProvider)
            : base("GroupMember.Read.All", graphServiceProvider)
        {
            _configuration = configuration;
            _groupId = ConfigUtil.GetConfigValueAndThrowIfEmpty(_configuration, ConfigConstants.EMPLOYEE_GROUP_ID);
        }

        public async Task<Dictionary<string, AzureUserDto>> SearchAsync(string search, int limit, CancellationToken cancellationToken = default)
        {
            return await SearchInternalAsync(_groupId, search, limit, cancellationToken);
        }
    }
}
