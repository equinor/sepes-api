using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Azure.Util;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Automapper
{
    public class StorageAccountExternalLinkResolver : IValueResolver<Dataset, StudySpecificDatasetDto, string>
    {
        public readonly IConfiguration _config;
        public StorageAccountExternalLinkResolver(IConfiguration config)
        {
            this._config = config;
        }

        public string Resolve(Dataset source, StudySpecificDatasetDto destination, string destMember, ResolutionContext context)
        {
            string storageAccountIdToUse = null;

            if (source.StudySpecific)
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
