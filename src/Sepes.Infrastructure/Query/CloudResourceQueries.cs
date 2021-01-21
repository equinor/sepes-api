using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Query
{
    public static class CloudResourceQueries
    {
        public static async Task<CloudResource> GetResourceGroupEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSingleResourceEntry(db, sandboxId, AzureResourceType.ResourceGroup);
        }

        public static async Task<CloudResource> GetDiagStorageAccountEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSingleResourceEntry(db, sandboxId, AzureResourceType.StorageAccount);
        }

        public static async Task<CloudResource> GetNetworkEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSingleResourceEntry(db, sandboxId, AzureResourceType.VirtualNetwork);
        }

        static IQueryable<CloudResource> WithBasicIncludesQueryable(SepesDbContext db)
        {
            return db.CloudResources
                .Include(r => r.Operations)
                .Include(sr=> sr.Sandbox)
                    .ThenInclude(sb=> sb.Study)
                        .ThenInclude(s=> s.StudyParticipants);
        }

        public static async Task<CloudResource> GetSingleResourceEntry(SepesDbContext db, int sandboxId, string resourceType)
        {
            var resourceEntry = await WithBasicIncludesQueryable(db).SingleOrDefaultAsync(r => r.SandboxId == sandboxId && r.ResourceType == resourceType && r.SandboxControlled);

            if (resourceEntry == null)
            {
                throw new Exception($"Could not locate Sandbox Resource database entry for type {resourceType}");
            }

            return resourceEntry;
        }

        public static IQueryable<CloudResource> GetResourcesByType(SepesDbContext db, int sandboxId, string resourceType, bool includeDeletedIfOperationNotFinished = false)
        {
            var resourceQuerable =
                WithBasicIncludesQueryable(db)
                .Where(r => r.SandboxId == sandboxId
                && r.ResourceType == resourceType
                && (!r.DeletedAt.HasValue ||
                
                (r.DeletedAt.HasValue && includeDeletedIfOperationNotFinished && r.Operations.Where(o => o.OperationType == CloudResourceOperationType.DELETE && o.Status == CloudResourceOperationState.DONE_SUCCESSFUL).Any() == false)));            


            return resourceQuerable;
        }

        public static IQueryable<CloudResource> GetResource(SepesDbContext db, int resourceId)
        {
            var resourceQuerable =
                WithBasicIncludesQueryable(db)
                .Where(r => r.Id == resourceId            
                && (!r.DeletedAt.HasValue || (r.DeletedAt.HasValue && r.Operations.Where(o => o.OperationType == CloudResourceOperationType.DELETE && o.Status == CloudResourceOperationState.DONE_SUCCESSFUL).Any() == false)));

            return resourceQuerable;
        }

        public static IQueryable<CloudResource> GetSandboxVirtualMachines(SepesDbContext db, int sandboxId)
        {
            return GetResourcesByType(db, sandboxId, AzureResourceType.VirtualMachine);           
        }

        public static async Task<List<CloudResource>> GetSandboxVirtualMachinesList(SepesDbContext db, int sandboxId)
        {
            var queryable = GetSandboxVirtualMachines(db, sandboxId);

            return await queryable.ToListAsync();
        }

        public static IQueryable<CloudResource> GetSandboxResourcesQueryable(SepesDbContext db, int sandboxId)
        {
            var queryable = WithBasicIncludesQueryable(db).Where(sr=> sr.SandboxId == sandboxId);

            return queryable;
        }

        public static async Task<int> GetCreateOperationIdForBastion(SepesDbContext db, int sandboxId)
        {
            var bastionResourceForSandbox = await WithBasicIncludesQueryable(db).SingleOrDefaultAsync(r => r.SandboxId == sandboxId && r.ResourceType == AzureResourceType.Bastion);

            if (bastionResourceForSandbox == null)
            {
                throw new Exception($"Could not locate Bastion resource entry");
            }

            if (bastionResourceForSandbox.DeletedAt.HasValue || bastionResourceForSandbox.Operations.FirstOrDefault(o => o.OperationType == CloudResourceOperationType.DELETE) != null)
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
