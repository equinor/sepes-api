﻿using Microsoft.EntityFrameworkCore;
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
    public static class SandboxResourceQueries
    {
        public static async Task<SandboxResource> GetResourceGroupEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSingleSandboxResourceEntry(db, sandboxId, AzureResourceType.ResourceGroup);
        }

        public static async Task<SandboxResource> GetDiagStorageAccountEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSingleSandboxResourceEntry(db, sandboxId, AzureResourceType.StorageAccount);
        }

        public static async Task<SandboxResource> GetNetworkEntry(SepesDbContext db, int sandboxId)
        {
            return await GetSingleSandboxResourceEntry(db, sandboxId, AzureResourceType.VirtualNetwork);
        }

        public static async Task<SandboxResource> GetSingleSandboxResourceEntry(SepesDbContext db, int sandboxId, string resourceType)
        {
            var resourceEntry = await db.SandboxResources.Include(r => r.Operations).SingleOrDefaultAsync(r => r.SandboxId == sandboxId && r.ResourceType == resourceType && r.SandboxControlled);

            if (resourceEntry == null)
            {
                throw new Exception($"Could not locate Sandbox Resource database entry for type {resourceType}");
            }

            return resourceEntry;
        }

        public static IQueryable<SandboxResource> GetSandboxResourcesByType(SepesDbContext db, int sandboxId, string resourceType, bool includeDeletedIfOperationNotFinished = false)
        {
            var resourceQuerable = db
                .SandboxResources
                .Include(r => r.Operations)
                .Where(r => r.SandboxId == sandboxId
                && r.ResourceType == resourceType
                && (!r.Deleted.HasValue ||
                
                (r.Deleted.HasValue && includeDeletedIfOperationNotFinished && r.Operations.Where(o => o.OperationType == CloudResourceOperationType.DELETE && o.Status == CloudResourceOperationState.DONE_SUCCESSFUL).Any() == false)));            


            return resourceQuerable;
        }

        public static IQueryable<SandboxResource> GetSandboxResource(SepesDbContext db, int resourceId)
        {
            var resourceQuerable = db
                .SandboxResources
                .Include(r => r.Sandbox)
                .Include(r => r.Operations)
                .Where(r => r.Id == resourceId            
                && (!r.Deleted.HasValue || (r.Deleted.HasValue && r.Operations.Where(o => o.OperationType == CloudResourceOperationType.DELETE && o.Status == CloudResourceOperationState.DONE_SUCCESSFUL).Any() == false)));

            return resourceQuerable;
        }

        public static IQueryable<SandboxResource> GetSandboxVirtualMachines(SepesDbContext db, int sandboxId)
        {
            return GetSandboxResourcesByType(db, sandboxId, AzureResourceType.VirtualMachine);           
        }

        public static async Task<List<SandboxResource>> GetSandboxVirtualMachinesList(SepesDbContext db, int sandboxId)
        {
            var queryable = GetSandboxVirtualMachines(db, sandboxId);

            return await queryable.ToListAsync();
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
