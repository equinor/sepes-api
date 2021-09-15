using Sepes.Common.Dto.VirtualMachine;
using System;
using System.Collections.Generic;

namespace Sepes.Tests.Common.ModelFactory.VirtualMachine
{
    public static class CreateVmDtoFactory
    {
        public static VirtualMachineCreateDto New(
            
            string vmName,
            string size = "Standard_F1",
            string os = "1",
            string username = "integrationTestUsr" ) {
         
            var result = new VirtualMachineCreateDto()
            {
                Name = vmName,
                Size = size,
                OperatingSystem = os,
                Username = username,
                Password = $"rR!1{Guid.NewGuid().ToString().Replace("-", "")}",
                DataDisks = new List<string>() { "standardssd-e1", "standardssd-e2" },               
            };

            return result;
        }
    }
}
