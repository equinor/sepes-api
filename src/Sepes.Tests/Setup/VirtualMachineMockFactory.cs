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
    public class VirtualMachineMockFactory
    {     

        public static IVirtualMachineService GetVirtualMachineService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var config = serviceProvider.GetService<IConfiguration>();
            var logger = serviceProvider.GetService<ILogger<VirtualMachineService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var sandboxModelServiceMock = new Mock<ISandboxModelService>();                

            var virtualMachineSizeService = new Mock<IVirtualMachineSizeService>();        

            var sandboxResourceCreateService = new Mock<ICloudResourceCreateService>();

            var sandboxResourceUpdateService = new Mock<ICloudResourceUpdateService>();

            var sandboxResourceDeleteService = new Mock<ICloudResourceDeleteService>();

            var sandboxResourceService = new Mock<ICloudResourceReadService>();

            var workQueue = new Mock<IProvisioningQueueService>();

            var azureVmService = new Mock<IAzureVirtualMachineExtenedInfoService>();

            return new VirtualMachineService(logger, config, db, mapper, userService.Object, sandboxModelServiceMock.Object, virtualMachineSizeService.Object, 
                sandboxResourceCreateService.Object, sandboxResourceUpdateService.Object, sandboxResourceDeleteService.Object,
                sandboxResourceService.Object, workQueue.Object, azureVmService.Object);
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
