using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Azure.Util;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Automapper
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
