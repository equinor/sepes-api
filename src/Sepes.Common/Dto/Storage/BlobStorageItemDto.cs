using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Common.Dto.Storage
{
    public class BlobStorageItemDto
    {
        public string Name { get; set; }

        public string ContentType { get; set; }

        public long Size { get; set; }

        public long? Modified { get; set; }

        public string Key { get; set; }
    }
}
