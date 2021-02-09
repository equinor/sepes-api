using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class AvailableDatasetResponseDto
    {
        public AvailableDatasetResponseDto(IEnumerable<AvailableDatasetDto> availableDatasets)
        {
            AvailableDatasets = availableDatasets;
        }

        public string Classification { get; set; }
        public string RestrictionDisplayText { get; set; }
        public IEnumerable<AvailableDatasetDto> AvailableDatasets { get; set; } 
    }

    public class AvailableDatasetDto
    {
        public int DatasetId { get; set; }

        public string Name { get; set; }

        public string Classification { get; set; }

        public bool AddedToSandbox { get; set; }
    }
}
