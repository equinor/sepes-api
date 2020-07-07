using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceService : ICloudResourceService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public CloudResourceService(SepesDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CloudResourceDto> Add(string resourceGroupId, string resourceGroupName, string type, string resourceId, string resourceName)
        {
            var newResource = new CloudResource()
            {
                ResourceGroupId = resourceGroupId,
                ResourceGroupName = resourceGroupName,
                ResourceType = type,
                ResourceName = resourceName,
                Status = ""
            };

            _db.CloudResources.Add(newResource);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(newResource.Id);
        }

        public async Task<CloudResourceDto> AddResourceGroup(string resourceGroupId, string resourceGroupName, string type)
        {
            return await Add(resourceGroupId, resourceGroupName, type, resourceGroupId, resourceGroupName);
        }

        public async Task<IEnumerable<DatasetListItemDto>> GetDatasetsLookupAsync()
        {
            var datasetsFromDb = await _db.Datasets
                .Where(ds => ds.StudyNo == null)
                .ToListAsync();
            var dataasetsDtos = _mapper.Map<IEnumerable<DatasetListItemDto>>(datasetsFromDb);

            return dataasetsDtos;
        }

        public async Task<CloudResourceDto> GetByIdAsync(int id)
        {
            var entityFromDb = await GetOrThrowAsync(id);

            var dto = MapEntityToDto(entityFromDb);

            return dto;
        }

        CloudResourceDto MapEntityToDto(CloudResource entity)
        {
            return _mapper.Map<CloudResourceDto>(entity);
        }

        async Task<CloudResource> GetOrThrowAsync(int id)
        {
            var entityFromDb = await _db.CloudResources.FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("AzureResource", id);
            }

            return entityFromDb;
        }

        public async Task<CloudResourceDto> MarkAsDeletedByIdAsync(int id)
        {
            var resourceFromDb = await MarkAsDeletedByIdInternalAsync(id);
            return MapEntityToDto(resourceFromDb);
        }

        async Task<CloudResource> MarkAsDeletedByIdInternalAsync(int id)
        {
            //WE DON*T REALLY DELETE FROM THIS TABLE, WE "MARK AS DELETED" AND KEEP THE RECORDS FOR FUTURE REFERENCE

            var entityFromDb = await _db.CloudResources.FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("AzureResource", id);
            }

            entityFromDb.DeletedBy = "TODO:AddUsernameHere";
            entityFromDb.DeletedFromAzure = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return entityFromDb;
        }
    }
}
