namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class AvailableDatasetDto
    {
        public int DatasetId { get; set; }

        public string Name { get; set; }

        public string Classification { get; set; }

        public bool AddedToSandbox { get; set; }
    }
}
