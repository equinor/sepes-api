namespace Sepes.Infrastructure.Dto
{
    public class ProvisioningQueueChildDto
    {
        public int SandboxResourceId { get; set; }

        public int SandboxResourceOperationId { get; set; }

        public string OperationType { get; set; }

        public string Status { get; set; }

        public int TryCount { get; set; }

        public int MaxTryCount { get; set; }  
        

        public string Description { get; set; }     
    }
}
