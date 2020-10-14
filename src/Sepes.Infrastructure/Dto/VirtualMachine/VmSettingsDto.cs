namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class VmSettingsDto
    {
        public string DiagnosticStorageAccountName { get; set; }

        public string NetworkName { get; set; }

        public string SubnetName { get; set; }

        public string PerformanceProfile { get; set; }

        public string OperatingSystem { get; set; }

        public string Distro{ get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
