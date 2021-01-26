using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class StorageAccountResourceExternalLinkResolver : IValueResolver<CloudResource, IHasStorageAccountLink, string>
    {
        public readonly IConfiguration _config;
        public StorageAccountResourceExternalLinkResolver(IConfiguration config)
        {
            this._config = config;
        }       

        public string Resolve(CloudResource source, IHasStorageAccountLink destination, string destMember, ResolutionContext context)
        {
            return AzureResourceUtil.CreateResourceLink(_config, source.ResourceId);
        }
    }   
}
