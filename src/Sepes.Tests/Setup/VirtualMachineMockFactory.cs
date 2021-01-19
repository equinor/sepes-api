using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Tests.Setup
{
    public class VirtualMachineMockFactory
    {
        public static IVirtualMachineLookupService GetVirtualMachineLookupService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<VirtualMachineService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var sandboxServiceMock = new Mock<ISandboxService>();
            sandboxServiceMock.Setup(x => x.CreateAsync(1, It.IsAny<SandboxCreateDto>()));

            var costServiceMock = new Mock<IAzureCostManagementService>();
            sandboxServiceMock.Setup(x => x.CreateAsync(1, It.IsAny<SandboxCreateDto>()));

            return new VirtualMachineLookupService(logger, db, mapper, sandboxServiceMock.Object, costServiceMock.Object);
        }
    }
}
