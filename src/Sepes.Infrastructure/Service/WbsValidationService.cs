using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Constants;
using Sepes.Common.Service;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WbsValidationService : RestApiServiceBase, IWbsValidationService
    {
        protected readonly string _subscriptionId;        

        public WbsValidationService(IConfiguration config, ILogger<WbsValidationService> logger, ITokenAcquisition tokenAcquisition)
          : base(config, logger, tokenAcquisition)
        {
            _subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];
        } 
        
        public async Task<bool> Exists(string wbsCode)
        {

        }

        
    }  
}