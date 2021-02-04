using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.DataModelService;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Tests.Setup
{
    public static class StudyServiceMockFactory
    {
        public static IStudyModelService CreateStudyModelService(ServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var db = serviceProvider.GetService<SepesDbContext>();
            var logger = serviceProvider.GetService<ILogger<StudyModelService>>();       
     
            var userService = UserFactory.GetUserServiceMockForAdmin(1);     

            return new StudyModelService(config, db, logger, userService.Object);
        }


        public static IStudyReadService CreateReadService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyReadService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = CreateStudyModelService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyReadService(db, mapper, logger, userService.Object, studyModelService, logoServiceMock.Object);
        }

        public static IStudyCreateUpdateService CreateUpdateService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyCreateUpdateService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = CreateStudyModelService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyCreateUpdateService(db, mapper, logger, userService.Object, studyModelService, logoServiceMock.Object);
        }

        public static IStudyDeleteService DeleteService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyDeleteService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelService = CreateStudyModelService(serviceProvider);

            var studySpecificDatasetService = DatasetServiceMockFactory.GetStudySpecificDatasetService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyDeleteService(db, mapper, logger, userService.Object, studyModelService, logoServiceMock.Object, studySpecificDatasetService);
        }
    }
}
