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
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Setup
{
    public static class StudyServiceMockFactory
    {
        public static IStudyModelService StudyModelService(ServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var db = serviceProvider.GetService<SepesDbContext>();
            var logger = serviceProvider.GetService<ILogger<StudyModelService>>();       
     
            var userService = UserFactory.GetUserServiceMockForAdmin(1);     

            return new StudyModelService(config, db, logger, userService.Object);
        }


        public static IStudyReadService ReadService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyReadService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = StudyModelService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyReadService(db, mapper, logger, userService.Object, studyModelService, logoServiceMock.Object);
        }

        public static IStudyCreateService CreateService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyCreateService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = StudyModelService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            var dsCloudResourceServiceMock = new Mock<IDatasetCloudResourceService>();
            dsCloudResourceServiceMock.Setup(x => x.CreateResourceGroupForStudySpecificDatasetsAsync(It.IsAny<Study>(), default(CancellationToken))).Returns(Task.CompletedTask);

            return new StudyCreateService(db, mapper, logger, userService.Object, studyModelService, logoServiceMock.Object, dsCloudResourceServiceMock.Object);
        }

        public static IStudyUpdateService UpdateService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyUpdateService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = StudyModelService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyUpdateService(db, mapper, logger, userService.Object, studyModelService, logoServiceMock.Object);
        }

        public static IStudyDeleteService DeleteService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyDeleteService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = StudyModelService(serviceProvider);

            var studySpecificDatasetService = DatasetServiceMockFactory.GetStudySpecificDatasetService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyDeleteService(db, mapper, logger, userService.Object, studyModelService, logoServiceMock.Object, studySpecificDatasetService);
        }
    }
}
