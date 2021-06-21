using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WbsValidationService : IWbsValidationService
    {
        readonly IConfiguration _configuration;
        readonly IUserService _userService;
        readonly IWbsApiService _wbsApiService;
        readonly IWbsCodeCacheModelService _wbsCodeCacheModelService;

        public WbsValidationService(IConfiguration configuration, IUserService userService, IWbsApiService wbsApiService, IWbsCodeCacheModelService wbsCodeCacheModelService)
        {
            _configuration = configuration;
            _userService = userService;
            _wbsApiService = wbsApiService;
            _wbsCodeCacheModelService = wbsCodeCacheModelService;
        }

        public async Task<bool> IsValidWithAccessCheck(string wbsCode, CancellationToken cancellation = default)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            OperationAccessUtil.HasAccessToOperationOrThrow(currentUser, UserOperation.Study_Create);
            return await IsValid(wbsCode, cancellation);
        }

        public async Task<bool> IsValid(string wbsCode, CancellationToken cancellation = default)
        {
            if (WbsValidationDisabled())
            {
                return true;
            }

            if (_userService.IsMockUser())
            {
                return true;
            }

            if (await _wbsCodeCacheModelService.ExistsAndValid(wbsCode, cancellation)) //Found in cache, means its valid
            {
                return true;
            }

            if (await _wbsApiService.Exists(wbsCode, cancellation)) //Found in api, means its valid
            {
                await _wbsCodeCacheModelService.Add(wbsCode, true);
                return true;
            }
            
            await _wbsCodeCacheModelService.Add(wbsCode, false);
            return false;
        }

        bool WbsValidationDisabled()
        {
            var valueFromConfig = ConfigUtil.GetBoolConfig(_configuration, ConfigConstants.WBS_DISABLE_ALL_VALIDATION);

            return valueFromConfig;
        }
    }
}