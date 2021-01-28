using Sepes.Infrastructure.Interface;

namespace Sepes.Infrastructure.Dto.Dataset
{
    public class DatasetResourceLightDto : IHasStorageAccountLink
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string LinkToExternalSystem { get; set; }

        public string RetryLink { get; set; }
    }
}
