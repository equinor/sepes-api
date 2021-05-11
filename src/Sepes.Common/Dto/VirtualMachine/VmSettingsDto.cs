using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class VmSettingsDto
    {
        public string DiagnosticStorageAccountName { get; set; }

        public string NetworkName { get; set; }

        public string SubnetName { get; set; }

        public string Size { get; set; }

        public List<string> DataDisks { get; set; }

        public string OperatingSystem { get; set; }

        public string OperatingSystemCategory { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public List<VmRuleDto> Rules { get; set; }
    }
}
