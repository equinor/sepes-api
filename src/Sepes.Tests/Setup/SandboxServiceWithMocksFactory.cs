using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Setup
{

    public static class SandboxServiceWithMocksFactory
    {
        public static ISandboxService Create(ServiceProvider serviceProvider, string userAppRole, int userId, SepesDbContext db = null)
        {
            if(db == null)
            {
                db = serviceProvider.GetService<SepesDbContext>();
            }

            var config = serviceProvider.GetService<IConfiguration>();
          
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<SandboxService>>();

            var userService = UserFactory.GetUserServiceMockForAppRole(userAppRole, userId);       

            var studyServiceMock = StudyServiceMockFactory.Create(serviceProvider);

            var sandboxCloudResourceServiceMock = new Mock<ISandboxCloudResourceService>();
            sandboxCloudResourceServiceMock.Setup(x => x.MakeDatasetsAvailable(It.IsAny<int>(), default(CancellationToken))).Returns(default(Task));

            return new SandboxService(config, db, mapper, logger, userService.Object, studyServiceMock, sandboxCloudResourceServiceMock.Object);
        }      
    }
}
