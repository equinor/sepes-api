using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Query
{
    public static class SandboxResourceQueries
    {
        public static async Task<SandboxResource> GetResourceGroupEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSandboxResourceEntry(db, sandboxId, AzureResourceType.ResourceGroup);
        }

        public static async Task<SandboxResource> GetDiagStorageAccountEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSandboxResourceEntry(db, sandboxId, AzureResourceType.StorageAccount);
        }

        public static async Task<SandboxResource> GetNetworkEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSandboxResourceEntry(db, sandboxId, AzureResourceType.VirtualNetwork);
        }

        public static async Task<SandboxResource> GetSandboxResourceEntry(SepesDbContext db, int sandboxId, string resourceType)
        {
            var resourceEntry = await db.SandboxResources.Include(r => r.Operations).SingleOrDefaultAsync(r => r.SandboxId == sandboxId && r.ResourceType == resourceType);

            if (resourceEntry == null)
            {
                throw new Exception($"Could not locate Sandbox Resource database entry for type {resourceType}");
            }

            return resourceEntry;
        }

        public static async Task<int> GetCreateOperationIdForBastion(SepesDbContext db, int sandboxId)
        {
            var bastionResourceForSandbox = await db.SandboxResources.Include(r => r.Operations).SingleOrDefaultAsync(r => r.SandboxId == sandboxId && r.ResourceType == AzureResourceType.Bastion);

            if (bastionResourceForSandbox == null)
            {
                throw new Exception($"Could not locate Bastion resource entry");
            }

            if (bastionResourceForSandbox.Deleted.HasValue || bastionResourceForSandbox.Operations.FirstOrDefault(o => o.OperationType == CloudResourceOperationType.DELETE) != null)
            {
                throw new Exception($"Bastion resource entry ({bastionResourceForSandbox.Id}) is marked for deletion");
            }

            var latestOperationEntry = bastionResourceForSandbox.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

            if (latestOperationEntry.OperationType == CloudResourceOperationType.UPDATE)
            {
                //TODO: Add additional checks? 
                return latestOperationEntry.Id;
            }
            else if (latestOperationEntry.OperationType == CloudResourceOperationType.CREATE)
            {
                return latestOperationEntry.Id;
            }

            throw new Exception($"Could not locate suitable Bastion resource operation entry");
        }
    }
}
