using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.DataModelService;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Setup
{
    public static class StudyServiceMockFactory
    {
        public static IStudyRawQueryModelService StudyRawQueryModelService(ServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();         
            var logger = serviceProvider.GetService<ILogger<StudyRawQueryModelService>>();           
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            return new StudyRawQueryModelService(config, logger, userService.Object);
        }

        public static IStudyEfModelService StudyEfModelService(ServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var db = serviceProvider.GetService<SepesDbContext>();
            var logger = serviceProvider.GetService<ILogger<StudyEfModelService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);     

            return new StudyEfModelService(config, db, logger, userService.Object);
        }

        public static IStudyRawQueryReadService StudyRawQueryReadService(ServiceProvider serviceProvider)
        {
            var studyRawModelService = StudyRawQueryModelService(serviceProvider);

            var logoReadServiceMock = new Mock<IStudyLogoReadService>();

            return new StudyRawQueryReadService(logoReadServiceMock.Object, studyRawModelService);
        }

        public static IStudyEfReadService StudyEfReadService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyEfReadService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);
            var studyEfModelService = StudyEfModelService(serviceProvider);
            var logoReadServiceMock = new Mock<IStudyLogoReadService>();

            return new StudyEfReadService(db, mapper, logger, userService.Object, studyEfModelService, logoReadServiceMock.Object);
        }

        public static IStudyCreateService CreateService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyCreateService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = StudyEfModelService(serviceProvider);
           
            var logoCreateServiceMock = new Mock<IStudyLogoCreateService>();
            var logoReadServiceMock = new Mock<IStudyLogoReadService>();

            var studyWbsValidationService = new Mock<IStudyWbsValidationService>();       

            var dsCloudResourceServiceMock = new Mock<IDatasetCloudResourceService>();
            dsCloudResourceServiceMock.Setup(x => x.CreateResourceGroupForStudySpecificDatasetsAsync(It.IsAny<Study>(), default(CancellationToken))).Returns(Task.CompletedTask);

            return new StudyCreateService(db, mapper, logger, userService.Object, studyModelService, logoCreateServiceMock.Object, logoReadServiceMock.Object, dsCloudResourceServiceMock.Object, studyWbsValidationService.Object);
        }

        public static IStudyUpdateService UpdateService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyUpdateService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = StudyEfModelService(serviceProvider);

            var logoReadServiceMock = new Mock<IStudyLogoReadService>();
            var logoCreateServiceMock = new Mock<IStudyLogoCreateService>();
            var logoDeleteServiceMock = new Mock<IStudyLogoDeleteService>();

            var studyWbsValidationService = new Mock<IStudyWbsValidationService>();

            return new StudyUpdateService(db, mapper, logger, userService.Object, studyModelService, logoReadServiceMock.Object, logoCreateServiceMock.Object, logoDeleteServiceMock.Object , studyWbsValidationService.Object);
        }

        public static IStudyDeleteService DeleteService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyDeleteService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyEfModelService = StudyEfModelService(serviceProvider);

            var studySpecificDatasetService = DatasetServiceMockFactory.GetStudySpecificDatasetService(serviceProvider);

            var logoReadServiceMock = new Mock<IStudyLogoReadService>();
            var logoDeleteServiceMock = new Mock<IStudyLogoDeleteService>();

            var resourceReadServiceMock = new Mock<ICloudResourceReadService>();
            //Todo: add some real resources to delete
            resourceReadServiceMock.Setup(service => service.GetSandboxResourcesForDeletion(It.IsAny<int>())).ReturnsAsync(new List<CloudResource>());

            return new StudyDeleteService(db, mapper, logger, userService.Object,
                studyEfModelService,
                logoReadServiceMock.Object,
                logoDeleteServiceMock.Object,
                studySpecificDatasetService,
                resourceReadServiceMock.Object);
        }
    }
}
