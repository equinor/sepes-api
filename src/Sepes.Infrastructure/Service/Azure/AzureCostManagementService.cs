using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureCostManagementService : AzureApiServiceBase, IAzureCostManagementService
    {
        public AzureCostManagementService(IConfiguration config, ILogger<AzureCostManagementService> logger, ITokenAcquisition tokenAcquisition) : base(config, logger, tokenAcquisition)
        {
        }

        public async Task<double> GetVmPrice(string region, string size)
        {
            var token = await AquireTokenAsync();

            return (double)0;
        }
    }
}
