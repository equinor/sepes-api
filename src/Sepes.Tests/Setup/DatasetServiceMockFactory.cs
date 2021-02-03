using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
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

        public static IStudySpecificDatasetService GetStudySpecificDatasetService(ServiceProvider serviceProvider, List<Study> studies = null)
        {
            if(studies == null)
            {
                studies = new List<Study>();
            }

            var db = serviceProvider.GetService<SepesDbContext>();
            var config = serviceProvider.GetService<IConfiguration>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudySpecificDatasetService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var studyModelServiceMock = new Mock<IStudyModelService>();
            studyModelServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<UserOperation>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(studies.FirstOrDefault());
            studyModelServiceMock.Setup(x => x.GetByIdWithoutPermissionCheckAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(studies.FirstOrDefault());

            var dsCloudResourceServiceMock = new Mock<IDatasetCloudResourceService>();
            dsCloudResourceServiceMock.Setup(x => x.CreateResourcesForStudySpecificDatasetAsync(It.IsAny<Dataset>(), "192.168.1.1", default(CancellationToken))).Returns(default(Task));

            return new StudySpecificDatasetService(db, mapper, logger, userService.Object, studyModelServiceMock.Object, dsCloudResourceServiceMock.Object);
        }
    }
}
