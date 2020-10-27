using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureCostManagementService : AzureApiServiceBase, IAzureCostManagementService
    {
        public AzureCostManagementService(IConfiguration config, ILogger<AzureCostManagementService> logger) : base(config, logger)
        {
        }

        public async Task<double> GetVmPrice(string region, string size)
        {
            var token = await AquireToken();

            return (double)0;
        }
    }
}
