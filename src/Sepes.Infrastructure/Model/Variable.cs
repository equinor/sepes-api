using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Variable : UpdateableBaseModel
    {
        [MaxLength(64)]
        [Required(AllowEmptyStrings =false)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public int? Int1 { get; set; } 

        public int? Int2 { get; set; }

        public int? Int3 { get; set; }

        [MaxLength(256)]
        public string Str1 { get; set; }

        [MaxLength(256)]
        public string Str2 { get; set; }

        [MaxLength(256)]
        public string Str3 { get; set; }

        public bool? Bool1 { get; set; }

        public bool? Bool2 { get; set; }

        public bool? Bool3 { get; set; }
    }

    public static class VariableNames
    {
        public static readonly string VmTimeoutAndRetryCount = "VM Timeout and Retry";
        public static readonly string BastionTimeoutAndRetryCount = "Bastion Timeout and Retry";
    }
}
