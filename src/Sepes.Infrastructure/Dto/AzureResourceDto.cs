using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto
{
    public class AzureResourceDto : UpdateableBaseDto
    {
        public string ResourceType { get; set; }
    }
}
