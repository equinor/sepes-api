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
            string storageAccountIdToUse = null;

            if (source.StudyId.HasValue && source.StudyId.Value > 0)
            {
                var storageAccountResource = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(source);

                if (storageAccountResource != null)
                {
                    storageAccountIdToUse = storageAccountResource.ResourceId;
                }
            }
            else
            {
                storageAccountIdToUse = source.StorageAccountId;
            }

            return AzureResourceUtil.CreateResourceLink(_config, storageAccountIdToUse);
        }
    }
}
