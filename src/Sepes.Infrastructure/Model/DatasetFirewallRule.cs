﻿namespace Sepes.Infrastructure.Model
{
    public class DatasetFirewallRule : UpdateableBaseModel
    {
        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public string Address { get; set; }
    }
}
