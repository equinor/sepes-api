﻿using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WbsValidationService : IWbsValidationService
    {
        readonly IUserService _userService;
        readonly IWbsApiService _wbsApiService;
        readonly IWbsCodeCacheModelService _wbsCodeCacheModelService;

        public WbsValidationService(ILogger<WbsValidationService> logger, IUserService userService, IWbsApiService wbsApiService, IWbsCodeCacheModelService wbsCodeCacheModelService)
        {
            _userService = userService;
            _wbsApiService = wbsApiService;
            _wbsCodeCacheModelService = wbsCodeCacheModelService;
        }



        public async Task<bool> IsValidWithAccessCheck(string wbsCode, CancellationToken cancellation = default)
        {
            StudyAccessUtil.HasAccessToOperationOrThrow(await _userService.GetCurrentUserAsync(), UserOperation.Study_Create);
            return await IsValid(wbsCode, cancellation);
        }

        public async Task<bool> IsValid(string wbsCode, CancellationToken cancellation = default)
        {
            if (await _wbsCodeCacheModelService.Exists(wbsCode, cancellation)) //Found in cache, means its valid
            {
                return true;
            }

            if (await _wbsApiService.Exists(wbsCode, cancellation)) //Found in api, means its valid
            {
                await _wbsCodeCacheModelService.Add(wbsCode);
                return true;
            }

            return false;
        }
    }
}