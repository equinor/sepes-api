using System.Collections.Generic;

namespace Sepes.Azure.Dto
{
    public class AzureProviderResponse
    {
        public string Id { get; set; }
        public string Namespace { get; set; }
        public IList<ProviderResourceType> ResourceTypes { get; set; }
    }

    public class ProviderResourceType
    {
        public string ResourceType { get; set; }
        public IList<string> Locations { get; set; }
    }
}
