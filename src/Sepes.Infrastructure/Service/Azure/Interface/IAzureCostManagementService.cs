using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureCostManagementService
    {
        Task<double> GetVmPrice(string region, string size);
    }
}
