using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{

    public class VariableService : IVariableService
    {
        readonly ILogger _logger;
        readonly SepesDbContext _db;

        public VariableService(ILogger logger, SepesDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<Variable> GetByNameAsync(string name)
        {
            return await GetOrThrowAsync(name);
        }

        async Task<Variable> GetOrThrowAsync(string name)
        {
            var entityFromDb = await _db.Variables.FirstOrDefaultAsync(s => s.Name == name);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntityByName("Variable", name);
            }

            return entityFromDb;
        }
    }
}
