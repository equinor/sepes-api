using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetServiceBase : ServiceBase<Dataset>
    {
        protected readonly ILogger _logger;       

        public DatasetServiceBase(SepesDbContext db, IMapper mapper, ILogger logger, IUserService userService)
            : base(db, mapper, userService)
        {
            _logger = logger;         
        }      

        protected async Task SoftDeleteAsync(Dataset dataset)
        {
            await SoftDeleteUtil.MarkAsDeleted(dataset, _userService);           
            await _db.SaveChangesAsync();
        }

        protected async Task HardDeleteAsync(Dataset dataset)
        {
            dataset.StudyDatasets.Clear();
            _db.Datasets.Remove(dataset);
            await _db.SaveChangesAsync();
        }     

        protected void PerformUsualTestForPostedDatasets(DatasetDto datasetDto)
        {
            if (String.IsNullOrWhiteSpace(datasetDto.Name))
            {
                throw new ArgumentException($"Field Dataset.Name is required. Current value: {datasetDto.Name}");
            }
            if (String.IsNullOrWhiteSpace(datasetDto.Classification))
            {
                throw new ArgumentException($"Field Dataset.Classification is required. Current value: {datasetDto.Classification}");
            }
            if (String.IsNullOrWhiteSpace(datasetDto.Location))
            {
                throw new ArgumentException($"Field Dataset.Location is required. Current value: {datasetDto.Location}");
            }
        }      
    }
}
