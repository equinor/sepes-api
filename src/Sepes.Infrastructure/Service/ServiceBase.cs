using AutoMapper;
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
    public class ServiceBase<TModel> where TModel : BaseModel
    {
        protected readonly SepesDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly IUserService _userService;

        public ServiceBase(SepesDbContext db, IMapper mapper, IUserService userService)
        {
            _db = db;
            _mapper = mapper;
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
    }
}
