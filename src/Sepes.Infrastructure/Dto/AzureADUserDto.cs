using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto
{
    public class AzureADUserDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
        public string MobilePhone { get; set; }
        public string? Source { get; set; }
        public int DatabaseId { get; set; }
        public string? ObjectId { get; set; }
    }
}
