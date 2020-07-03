using System;

namespace Sepes.Infrastructure.Model
{
    public class AzureResource : UpdateableBaseModel
    {  
        public string ResourceId { get; set; }

        public string ResourceName { get; set; }

        public string ResourceType { get; set; }

        public string ResourceGroupId { get; set; }

        public string ResourceGroupName { get; set; }

        public string Status { get; set; }

        public DateTime DeletedFromAzure { get; set; }

        public string DeletedBy { get; set; }
    }    
}
