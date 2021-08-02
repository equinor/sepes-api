using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Common.Service
{
    public class RestApiTokenAquisitionWithIdentityService : IRestApiTokenAquisitionWithIdentityService
    {
        protected readonly ILogger _logger;
        protected readonly IConfiguration _config;
        protected readonly ITokenAcquisition _tokenAcquisition; 

        public RestApiTokenAquisitionWithIdentityService(IConfiguration config, ILogger<RestApiTokenAquisitionWithIdentityService> logger, ITokenAcquisition tokenAcquisition)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));      
        }

        public async Task<string> GetAccessTokenForAppAsync(string scope)
        {
            return await _tokenAcquisition.GetAccessTokenForAppAsync(scope);
        }

        public async Task<string> GetAccessTokenForUserAsync(IEnumerable<string> scopes)
        {
            return await _tokenAcquisition.GetAccessTokenForUserAsync(scopes: scopes);
        } 
    }
}