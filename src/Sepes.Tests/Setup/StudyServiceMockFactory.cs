using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Tests.Setup
{

    public static class StudyServiceMockFactory
    {
        public static IStudyService Create(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyService(db, mapper, logger, userService.Object, logoServiceMock.Object);
        }

        public static IStudyCreateUpdateService CreateUpdateService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);
            var studySpecificDatasetService = DatasetServiceMockFactory.GetStudySpecificDatasetService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyCreateUpdateService(db, mapper, logger, userService.Object, logoServiceMock.Object, studySpecificDatasetService);
        }

        public static IStudyDeleteService DeleteService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);
            var studySpecificDatasetService = DatasetServiceMockFactory.GetStudySpecificDatasetService(serviceProvider);

            var logoServiceMock = new Mock<IStudyLogoService>();

            return new StudyDeleteService(db, mapper, logger, userService.Object, logoServiceMock.Object, studySpecificDatasetService);
        }
    }
}
