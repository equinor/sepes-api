using Sepes.Common.Dto.Interfaces;

namespace Sepes.Common.Dto.Sandbox
{
    public class SandboxDatasetDto : IHasDataClassification
    {
        public int DatasetId { get; set; }

        public string Name { get; set; }
        public int StudyId { get; set; }

        public int SandboxId { get; set; }

        public string SandboxName { get; set; }

        public string Classification { get; set; }
    }
}
