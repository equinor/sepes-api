namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class CreateVmUserInputDto
    {
        public string Region { get; set; }

        public string Name { get; set; }

        public string PerformanceProfile { get; set; }

        public string OperatingSystem { get; set; }

        public string Distro { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
