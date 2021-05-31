using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.DataModelService;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Common.ServiceMockFactories.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Setup
{

    public static class DatasetServiceMockFactory
    {
        public static IPreApprovedDatasetModelService GetPreApprovedDatasetModelService(ServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var db = serviceProvider.GetService<SepesDbContext>();         
            var logger = serviceProvider.GetService<ILogger<PreApprovedDatasetModelService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var service = new PreApprovedDatasetModelService(config, db, logger, userService.Object);
            return service;
        }

        public static Mock<IPreApprovedDatasetModelService> GetPreApprovedDatasetModelService(List<Dataset> datasets)
        {
            var studyModelServiceMock = new Mock<IPreApprovedDatasetModelService>();
            studyModelServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<UserOperation>())).ReturnsAsync((int a, UserOperation b) => datasets.FirstOrDefault(s => s.Id == a));
            return studyModelServiceMock;
        }

        public static Mock<IStudySpecificDatasetModelService> GetStudySpecificDatasetModelService(List<Dataset> datasets)
        {
            var studyModelServiceMock = new Mock<IStudySpecificDatasetModelService>();
            studyModelServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<UserOperation>())).ReturnsAsync((int a, UserOperation b) => datasets != null ? datasets.FirstOrDefault(s => s.Id == a) : null);
            studyModelServiceMock.Setup(x => x.GetForResourceAndFirewall(It.IsAny<int>(), It.IsAny<UserOperation>())).ReturnsAsync((int a, UserOperation b) => datasets != null ? datasets.FirstOrDefault(s => s.Id == a) : null);
            studyModelServiceMock.Setup(x => x.GetByIdWithoutPermissionCheckAsync(It.IsAny<int>())).ReturnsAsync((int a) => datasets != null ? datasets.FirstOrDefault(s => s.Id == a) : null);
            return studyModelServiceMock;
        }

        public static IDatasetService GetDatasetService(ServiceProvider serviceProvider, List<Dataset> datasets = null)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<DatasetService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var preApprovedDatasetModelService = GetPreApprovedDatasetModelService(datasets);

            return new DatasetService(db, mapper, logger, userService.Object, preApprovedDatasetModelService.Object);
        }

        public static IStudyDatasetService GetStudyDatasetService(ServiceProvider serviceProvider, List<Study> studies = null, List<Dataset> datasets = null)
        {
            var db = serviceProvider.GetService<SepesDbContext>();          
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyDatasetService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);            

            var studyModelServiceMock = new Mock<IStudyModelService>();
            studyModelServiceMock.Setup(x => x.GetForDatasetsAsync(It.IsAny<int>(), It.IsAny<UserOperation>())).ReturnsAsync(( int a, UserOperation b) => studies != null ? studies.FirstOrDefault(s=> s.Id == a) : null);

            var studySpecificDatasetModelService = GetStudySpecificDatasetModelService(datasets);

            return new StudyDatasetService(db, mapper, logger, userService.Object, studyModelServiceMock.Object, studySpecificDatasetModelService.Object);          
        }

        public static IStudySpecificDatasetService GetStudySpecificDatasetService(ServiceProvider serviceProvider, List<Study> studies = null, List<Dataset> datasets = null)
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
            studyModelServiceMock.Setup(x => x.GetForDatasetsAsync(It.IsAny<int>(), It.IsAny<UserOperation>())).ReturnsAsync((int a, UserOperation b) => studies != null ? studies.FirstOrDefault(s => s.Id == a) : null);
            studyModelServiceMock.Setup(x => x.GetForDatasetCreationAsync(It.IsAny<int>(), It.IsAny<UserOperation>())).ReturnsAsync((int a, UserOperation b) => studies != null ? studies.FirstOrDefault(s => s.Id == a) : null);
            
            var dsCloudResourceServiceMock = new Mock<IDatasetCloudResourceService>();
            dsCloudResourceServiceMock.Setup(x => x.CreateResourcesForStudySpecificDatasetAsync(It.IsAny<Study>(), It.IsAny<Dataset>(), "192.168.1.1", default(CancellationToken))).Returns(default(Task));

            var studySpecificDatasetModelService = GetStudySpecificDatasetModelService(datasets);

            var studyWbsValidationService = StudyWbsValidationMockServiceFactory.GetService(serviceProvider, true, false);

            return new StudySpecificDatasetService(db, mapper, logger, userService.Object, studyModelServiceMock.Object, studyWbsValidationService, studySpecificDatasetModelService.Object, dsCloudResourceServiceMock.Object);
        }
    }
}
