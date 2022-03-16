using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class CloudResourceReadService : CloudResourceServiceBase, ICloudResourceReadService
    {
        public readonly IConfiguration _config;

        public CloudResourceReadService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceReadService> logger, IStudyPermissionService studyPermissionService)
         : base(db, config, mapper, logger, studyPermissionService)
        {
            _config = config;
        }

        public async Task<CloudResource> GetByIdNoAccessCheckAsync(int id)
        {
            return await GetInternalWithoutAccessCheckAsync(id);
        }

        public Task<CloudResource> GetByStudyIdForDeletionNoAccessCheckAsync(int id)
        {
            return _db.CloudResources.Where(r => r.StudyId == id && !r.Deleted).Include(r => r.ChildResources).FirstOrDefaultAsync();
        }

        public async Task<CloudResource> GetByIdAsync(int id, UserOperation operation)
        {
            return await GetInternalAsync(id, operation, throwIfNotFound: true);
        }

        public async Task<List<CloudResource>> GetAllActiveResources() => await _db.CloudResources.Include(sr => sr.Sandbox)
                                                                                                   .ThenInclude(sb => sb.Study)
                                                                                                    .Include(sr => sr.Operations)
                                                                                                   .Where(sr => !sr.Deleted)
                                                                                                   .ToListAsync();




        public async Task<IEnumerable<CloudResource>> GetDeletedResourcesAsync() => await _db.CloudResources.Include(sr => sr.Operations).Where(sr => sr.Deleted && sr.DeletedAt.HasValue && sr.DeletedAt.Value.AddMinutes(15) < DateTime.UtcNow)
                                                                                                                .ToListAsync();

        public async Task<bool> ResourceIsDeleted(int resourceId)
        {
            var resource = await _db.CloudResources.AsNoTracking().FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource == null)
            {
                return true;
            }

            return SoftDeleteUtil.IsMarkedAsDeleted(resource);
        }

        IQueryable<CloudResource> DatasetResourceGroupQueryable(int studyId)
        {
            return _db.CloudResources.Where(r => r.StudyId == studyId
             && r.Deleted == false
             && r.ResourceType == AzureResourceType.ResourceGroup
             && r.Purpose == CloudResourcePurpose.StudySpecificDatasetContainer);
        }

        public async Task<List<int>> GetDatasetResourceGroupIdsForStudy(int studyId)
        {
            var resourceGroupIdsQueryable =
             DatasetResourceGroupQueryable(studyId)
             .Select(r => r.Id);

            return await resourceGroupIdsQueryable.ToListAsync();
        }

        public async Task<List<int>> GetSandboxResourceGroupIdsForStudy(int studyId)
        {
            var resourceGroupsQueryable =
                _db.Sandboxes
                .Where(sb => sb.StudyId == studyId && sb.Deleted == false)
                .SelectMany(sb => sb.Resources)
                .Where(r => r.Deleted == false && r.ResourceType == AzureResourceType.ResourceGroup && (r.SandboxControlled || r.Purpose == CloudResourcePurpose.SandboxResourceGroup))
                .Select(r => r.Id);

            return await resourceGroupsQueryable.ToListAsync();
        }
        public async Task<List<int>> GetDatasetStorageAccountIdsForStudy(int studyId)
        {
            var datasetQueryable = DatasetResourceGroupQueryable(studyId)
             .SelectMany(r => r.ChildResources)
             .Where(r =>
             r.Deleted == false
             && r.ResourceType == AzureResourceType.StorageAccount
             && r.Purpose == CloudResourcePurpose.StudySpecificDatasetStorageAccount)
             .Select(r => r.Id);

            return await datasetQueryable.ToListAsync();
        }

        public async Task<List<CloudResource>> GetSandboxResourcesForDeletion(int sandboxId)
        {

            var queryable = _db.CloudResources
                .Include(r => r.Operations)
                .ThenInclude(o => o.DependsOnOperation)
                .Where(r => r.SandboxId == sandboxId && !r.Deleted);

            return await queryable.ToListAsync();
        }


    }
}
