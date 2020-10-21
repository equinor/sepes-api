namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class VmDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public string LastKnownProvisioningState { get; set; }

        public string OperatingSystem { get; set; }    

        public string Status { get; set; }         

        public string Created { get; set; }

        public string CreatedBy { get; set; }


        //Todo:       

        //Disks
        //Created, updated
        //Nic

    }
}
