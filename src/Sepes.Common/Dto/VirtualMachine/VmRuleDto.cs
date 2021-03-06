﻿namespace Sepes.Common.Dto.VirtualMachine
{
    public class VmRuleDto
    {
        public string Name { get; set; }        

        public RuleDirection Direction { get; set; }
       

        public string Description { get; set; }  
        public string Protocol { get; set; } //(dropdown med any(default), tcp, udp, icmp)
        public string Ip { get; set; }      
        public int Port { get; set; }// http (80), https(443), custom (any)       

        public RuleAction Action { get; set; } = RuleAction.Allow;

        public override string ToString()
        {
            return $"{Name}-{Ip}-{Port}-{Protocol}-{Direction}-{Action}";
        }
    }
}
