using Sepes.Common.Dto.Interfaces;

namespace Sepes.Infrastructure.Response.Sandbox
{
    public class AvailableDatasetItem : IHasDataClassification
    {
        public int DatasetId { get; set; }

        public string Name { get; set; }

        public string Classification { get; set; }

        public bool AddedToSandbox { get; set; }
    }
}
