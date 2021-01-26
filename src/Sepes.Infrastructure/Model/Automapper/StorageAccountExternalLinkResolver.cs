using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class StorageAccountExternalLinkResolver : IValueResolver<Dataset, DatasetDto, string>
    {
        public readonly IConfiguration _config;
        public StorageAccountExternalLinkResolver(IConfiguration config)
        {
            this._config = config;
        }       

        public string Resolve(Dataset source, DatasetDto destination, string destMember, ResolutionContext context)
        {
            return AzureResourceUtil.CreateResourceLink(_config, source.StorageAccountId);
        }
    }   
}
