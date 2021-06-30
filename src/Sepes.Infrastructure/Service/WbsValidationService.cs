using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WbsValidationService : IWbsValidationService
    {
        readonly IConfiguration _configuration;
        readonly IUserService _userService;
        readonly IOperationPermissionService _operationPermissionService;
        readonly IWbsApiService _wbsApiService;
        readonly IWbsCodeCacheModelService _wbsCodeCacheModelService;

        public WbsValidationService(IConfiguration configuration, IUserService userService, IOperationPermissionService operationPermissionService, IWbsApiService wbsApiService, IWbsCodeCacheModelService wbsCodeCacheModelService)
        {
            _configuration = configuration;
            _userService = userService;
            _operationPermissionService = operationPermissionService;
            _wbsApiService = wbsApiService;
            _wbsCodeCacheModelService = wbsCodeCacheModelService;
        }

        public async Task<bool> IsValidWithAccessCheck(string wbsCode, CancellationToken cancellation = default)
        {
            await _operationPermissionService.HasAccessToAnyOperationOrThrow(UserOperation.Study_Create, UserOperation.Study_Update_Metadata);
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

            var cachedItem = await _wbsCodeCacheModelService.Get(wbsCode);

            if (cachedItem != null) //Found in cache, means its valid
            {
                return cachedItem.Valid;
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