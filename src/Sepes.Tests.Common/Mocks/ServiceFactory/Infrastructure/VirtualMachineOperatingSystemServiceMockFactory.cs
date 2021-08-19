using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Sepes.Tests.Common.ServiceMockFactories.Infrastructure
{
    public static class VirtualMachineOperatingSystemServiceMockFactory
    {
        public static Mock<IVirtualMachineOperatingSystemService> GetService(bool willFail)
        {
            var wbsValidationServiceMock = new Mock<IVirtualMachineOperatingSystemService>();

            var availableOperatingSystemsLookup = new Dictionary<int, VmImageDto>() { 
                { 1, new VmImageDto() { Id = 1, Category = "windows", Name = "v1", ForeignSystemId = "ForeignSystemId", DisplayValue = "Windows Server Datacenter 2019", DisplayValueExtended = "Windows Server Datacenter 2019 (v1)", Recommended = true } } ,
                  { 2, new VmImageDto() { Id = 2, Category = "linux", Name = "v1", ForeignSystemId = "ForeignSystemId", DisplayValue = "SUSE Linux", DisplayValueExtended = "SUSE Linux Extended", Recommended = true } }
            };

            if (willFail)
            {
                wbsValidationServiceMock.Setup(s =>
                        s.GetImage(It.IsAny<int>()))
                    .ThrowsAsync(new Exception());
            }
            else
            {
                wbsValidationServiceMock.Setup(s =>
                        s.GetImage(It.IsAny<int>()))
                    .ReturnsAsync((int id) => { if (availableOperatingSystemsLookup.TryGetValue(id, out VmImageDto relevantImage)) { return relevantImage; } else return null; });
            }

            return wbsValidationServiceMock;
        }
    }
}
