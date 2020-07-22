using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceOperationService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public SandboxResourceOperationService(SepesDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<SandboxResourceOperationDto> Add(int sandboxResourceId, SandboxResourceOperationDto operationDto)
        {
            var sandboxResourceFromDb = await GetSandboxResourceOrThrowAsync(sandboxResourceId);
            var newOperation = _mapper.Map<SandboxResourceOperation>(operationDto);
            sandboxResourceFromDb.Operations.Add(newOperation);
            await _db.SaveChangesAsync();
            return await GetByIdAsync(newOperation.Id);
        }

        public async Task<SandboxResourceOperationDto> GetByIdAsync(int id)
        {
            var itemFromDb = await GetOrThrowAsync(id);
            var itemDto = _mapper.Map<SandboxResourceOperationDto>(itemFromDb);
            return itemDto;
        }     

        async Task<SandboxResourceOperation> GetOrThrowAsync(int id)
        {
            var entityFromDb = await _db.SandboxResourceOperations.FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("SandboxResourceOperation", id);
            }

            return entityFromDb;
        }

        public async Task<SandboxResourceOperationDto> UpdateStatus(int id, string status)
        {
            var itemFromDb = await GetOrThrowAsync(id);
            itemFromDb.Status = status;
            itemFromDb.Updated = DateTime.UtcNow;
            itemFromDb.TryCount++;
            await _db.SaveChangesAsync();

            return await GetByIdAsync(itemFromDb.Id);
        }

        private async Task<SandboxResource> GetSandboxResourceOrThrowAsync(int id)
        {
            var entityFromDb = await _db.SandboxResources
                .Include(sr => sr.Operations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("AzureResource", id);
            }

            return entityFromDb;
        }
    }
}
