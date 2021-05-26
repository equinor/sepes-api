using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using Sepes.Common.Response.Sandbox;
using Sepes.Tests.Common.ModelFactory;

namespace Sepes.Tests.Setup
{

    public static class SandboxServiceWithMocksFactory
    {
        public static ISandboxService ForSandboxCreate(ServiceProvider serviceProvider, string userAppRole, int userId, List<Study> studies = null, List<Sandbox> sandboxForSandboxDetails = null)
        {
            //STUDY MODEL SERVICE
            var studyModelServiceMock = new Mock<IStudyModelService>();
            
            studyModelServiceMock
                .Setup(x => 
                    x.GetForSandboxCreateAndDeleteAsync(It.IsAny<int>(), It.IsAny<UserOperation>()))
                .ReturnsAsync((int a, UserOperation b) => studies?.FirstOrDefault(s => s.Id == a));

            //SANDBOX MODEL SERVICE
            var sandboxModelServiceMock = new Mock<ISandboxModelService>();

            sandboxModelServiceMock
                .Setup(x =>
                    x.GetDetailsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int a) => sandboxForSandboxDetails?.FirstOrDefault(sd => sd.Id == a));

            return Create(serviceProvider, userAppRole, userId, studyModelServiceMock.Object,  sandboxModelServiceMock.Object);
        }
        
        public static ISandboxService Create(ServiceProvider serviceProvider, string userAppRole, int userId, IStudyModelService studyModelService, ISandboxModelService sandboxModelService)
        {
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<SandboxService>>();

            var userService = UserFactory.GetUserServiceMockForAppRole(userAppRole, userId);

            var sandboxResourceCreateServiceMock = new Mock<ISandboxResourceCreateService>();
            var sandboxResourceDeleteServiceMock = new Mock<ISandboxResourceDeleteService>();

            return new SandboxService(mapper, logger, userService.Object, studyModelService, sandboxModelService, sandboxResourceCreateServiceMock.Object, sandboxResourceDeleteServiceMock.Object);
        }      
    }
}
