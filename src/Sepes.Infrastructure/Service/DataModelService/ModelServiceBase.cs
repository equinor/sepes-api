using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class ModelServiceBase<TModel> where TModel : BaseModel
    {
        protected readonly SepesDbContext _db;       
        protected readonly ILogger _logger;
        protected readonly IUserService _userService;

        public ModelServiceBase(SepesDbContext db, ILogger logger, IUserService userService)
        {
            _db = db;        
            _logger = logger;
            _userService = userService;
        }

        public async Task<int> Add(TModel entity)
        {
            Validate(entity);

            var dbSet = _db.Set<TModel>();

            dbSet.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task Remove(TModel entity)
        {
            var dbSet = _db.Set<TModel>();

            dbSet.Remove(entity);
            await _db.SaveChangesAsync();       
        }       

        public bool Validate(TModel entity)
        {
            var validationErrors = new List<ValidationResult>();
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(entity, null, null);
            var isValid = Validator.TryValidateObject(entity, context, validationErrors);

            if (!isValid)
            {
                var errorBuilder = new StringBuilder();

                errorBuilder.AppendLine("Invalid data: ");

                foreach (var error in validationErrors)
                {
                    errorBuilder.AppendLine(error.ErrorMessage);
                }

                throw new ArgumentException(errorBuilder.ToString());
            }

            return true;          
        }

        protected async Task<Study> GetStudyByIdAsync(int studyId, UserOperation userOperation, bool withIncludes)
        {
            return await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, userOperation, withIncludes);
        }

        protected async Task<Sandbox> GetSandboxByIdNoChecksAsync(int sandboxId)
        {
            return await SandboxSingularQueries.GetSandboxByIdNoChecks(_db, sandboxId);
        }
    }
}
