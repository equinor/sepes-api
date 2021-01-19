using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Setup
{

    public static class DatasetServiceMockFactory
    {
        public static IDatasetService GetDatasetService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<DatasetService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);           

            return new DatasetService(db, mapper, logger, userService.Object);
        }

        public static IStudyDatasetService GetStudyDatasetService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var config = serviceProvider.GetService<IConfiguration>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyDatasetService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);         

            return new StudyDatasetService(config, db, mapper, logger, userService.Object);          
        }

        public static IStudySpecificDatasetService GetStudySpecificDatasetService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var config = serviceProvider.GetService<IConfiguration>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudySpecificDatasetService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var dsCloudResourceServiceMock = new Mock<IDatasetCloudResourceService>();
            dsCloudResourceServiceMock.Setup(x => x.CreateResourcesForStudySpecificDatasetAsync(It.IsAny<Study>(), It.IsAny<Dataset>(), "192.168.1.1", default(CancellationToken))).Returns(default(Task));

            return new StudySpecificDatasetService(db, mapper, logger, userService.Object, dsCloudResourceServiceMock.Object);
        }

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
