using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
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

        public async Task Add(SandboxResourceOperationDto operationDto)
        {
            var newOperation = _mapper.Map<SandboxResourceOperation>(operationDto);
            _db.SandboxResourceOperations.Add(newOperation);
            await _db.SaveChangesAsync();
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
            await _db.SaveChangesAsync();

            return await GetByIdAsync(itemFromDb.Id);
        }


    }
}
