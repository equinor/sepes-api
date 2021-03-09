using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class ModelServiceBase<TModel> where TModel : BaseModel
    {
        protected readonly IConfiguration _configuration;
        protected readonly SepesDbContext _db;       
        protected readonly ILogger _logger;
        protected readonly IUserService _userService;

        public ModelServiceBase(IConfiguration configuration, SepesDbContext db, ILogger logger, IUserService userService)
        {
            _configuration = configuration;
            _db = db;        
            _logger = logger;
            _userService = userService;
        }

        protected string GetDbConnectionString()
        {
            return _db.Database.GetDbConnection().ConnectionString;        
        }

        public async Task<TModel> AddAsync(TModel entity)
        {
            Validate(entity);

            var dbSet = _db.Set<TModel>();

            dbSet.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
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
            var context = new ValidationContext(entity, null, null);
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

        protected async Task CheckAccesAndThrowIfMissing(Study study, UserOperation operation)
        {
            await StudyAccessUtil.CheckAccesAndThrowIfMissing(_userService, study, operation);           
        }
    }
}
