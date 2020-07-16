using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureStorageAccountService
    {
        Task<IStorageAccount> CreateStorageAccount(Region region, string sandboxName, string resourceGroupName);
        Task<IStorageAccount> CreateDiagnosticsStorageAccount(Region region, string sandboxName, string resourceGroupName);
        Task DeleteStorageAccount(string resourceGroupName, string storageAccountName);
        Task<bool> Exists(string resourceGroupName, string storageAccountName);

        // CreateStorageContainer(type);
        // DeleteStoragecontainer(type);

        // Methods for accessing storageContainers:
    }
}
