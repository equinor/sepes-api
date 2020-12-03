using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetFileService
    {
       

        Task<List<Guid>> AddFiles(int datasetId, List<IFormFile> files);
    }
}
