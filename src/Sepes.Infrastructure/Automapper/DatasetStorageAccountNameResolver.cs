using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Automapper
{
    public class DatasetStorageAccountNameResolver : IValueResolver<Dataset, DatasetDto, string>
    {
        public readonly IConfiguration _config;
        public DatasetStorageAccountNameResolver(IConfiguration config)
        {
            this._config = config;
        }

        public string Resolve(Dataset source, DatasetDto destination, string destMember, ResolutionContext context)
        {
            if (source.StudySpecific)
            {
                var storageAccountResource = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(source);

                if (storageAccountResource != null)
                {
                    return storageAccountResource.ResourceName;
                }
            }
            else
            {
                return source.StorageAccountName;
            }

            return null;
        }
    }
}
