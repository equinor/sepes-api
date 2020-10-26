namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class VmDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public string LastKnownProvisioningState { get; set; }

        public string OperatingSystem { get; set; }  

        public string Size { get; set; }
      
        public int NumberOfCores { get; set; }       
    
        public int OsDiskSizeInMB { get; set; }
      
        public int ResourceDiskSizeInMB { get; set; }
   
        public int MemoryInMB { get; set; }

        public int MaxDataDiskCount { get; set; }

        public string Status { get; set; }         

        public string Created { get; set; }

        public string CreatedBy { get; set; }

        //Todo:  
        


        //Disks
        //Created, updated
        //Nic

    }
}
