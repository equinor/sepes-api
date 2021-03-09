using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Tests.Setup
{
    public class VirtualMachineMockServiceFactory
    {     

        public static IVirtualMachineCreateService GetVirtualMachineCreateService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var config = serviceProvider.GetService<IConfiguration>();
            var logger = serviceProvider.GetService<ILogger<VirtualMachineCreateService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var sandboxModelServiceMock = new Mock<ISandboxModelService>();            

            var resourceCreateService = new Mock<ICloudResourceCreateService>();

            var resourceUpdateService = new Mock<ICloudResourceUpdateService>();

            var resourceDeleteService = new Mock<ICloudResourceDeleteService>();

            var resourceReadService = new Mock<ICloudResourceReadService>();

            var workQueue = new Mock<IProvisioningQueueService>();

            var virtualMachineOperatingSystemService = new Mock<IVirtualMachineOperatingSystemService>();

            return new VirtualMachineCreateService(config, db, logger, mapper, userService.Object,
                sandboxModelServiceMock.Object,            
                resourceCreateService.Object,
                  resourceReadService.Object,
                resourceUpdateService.Object,
                resourceDeleteService.Object,              
                workQueue.Object,
                virtualMachineOperatingSystemService.Object);
        }

        public static IVirtualMachineValidationService GetVirtualMachineValidationService()
        { 
            return new VirtualMachineValidationService();
        }

        public static IVirtualMachineSizeService GetVirtualMachineSizeService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<VirtualMachineSizeService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var sandboxServiceMock = new Mock<ISandboxService>();
            sandboxServiceMock.Setup(x => x.CreateAsync(1, It.IsAny<SandboxCreateDto>()));

            var costServiceMock = new Mock<IAzureCostManagementService>();
            sandboxServiceMock.Setup(x => x.CreateAsync(1, It.IsAny<SandboxCreateDto>()));
            sandboxServiceMock.Setup(x => x.GetAsync(1, UserOperation.Study_Crud_Sandbox, false)).ReturnsAsync(new SandboxDto { Id = 1, Region = "norwayeast" });

            var sandboxModelServiceMock = new Mock<ISandboxModelService>();

            return new VirtualMachineSizeService(logger, db, mapper, sandboxModelServiceMock.Object);
        }
    }
}
