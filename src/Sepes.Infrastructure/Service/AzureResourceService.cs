using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceService : IAzureResourceService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public AzureResourceService(SepesDbContext db, IMapper mapper)
        {            
            _db = db;
            _mapper = mapper;
        }

        public async Task<AzureResourceDto> Add(string resourceGroupId, string resourceGroupName, string type, string resourceId, string resourceName)
        {
            var newResource = new AzureResource() {
                ResourceGroupId = resourceGroupId, 
                ResourceGroupName = resourceGroupName, 
                ResourceType = type, 
                ResourceName = resourceName, 
                Status = ""  };

            _db.AzureResources.Add(newResource);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(newResource.Id);
        }

        public async Task<AzureResourceDto> AddResourceGroup(string resourceGroupId, string resourceGroupName, string type)
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

        public async Task<AzureResourceDto> GetByIdAsync(int id)
        {
            var entityFromDb = await GetOrThrowAsync(id);

            var dto = _mapper.Map<AzureResourceDto>(entityFromDb);

            return dto;
        } 
        
        async Task<AzureResource> GetOrThrowAsync(int id)
        {
            var entityFromDb = await _db.AzureResources.FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("AzureResource", id);
            }

            return entityFromDb;
        }

        //public Task<StudyDto> UpdateDatasetAsync(int id, DatasetDto datasetToUpdate)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
