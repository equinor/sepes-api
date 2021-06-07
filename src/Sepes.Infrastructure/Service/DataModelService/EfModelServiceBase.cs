using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class EfModelServiceBase<TModel> : EfModelServiceBase where TModel : BaseModel
    {
        protected readonly IStudyPermissionService _studyPermissionService;

        public EfModelServiceBase(IConfiguration configuration, SepesDbContext db, ILogger logger, IStudyPermissionService studyPermissionService)
            : base(configuration, db, logger)
        {
            _studyPermissionService = studyPermissionService;
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

        protected string GetDbConnectionString()
        {
            return _db.Database.GetDbConnection().ConnectionString;
        }

        protected async Task CheckAccesAndThrowIfNotAllowed(Study study, UserOperation operation)
        {
            await _studyPermissionService.VerifyAccessOrThrow(study, operation);
        }       
    }
}
