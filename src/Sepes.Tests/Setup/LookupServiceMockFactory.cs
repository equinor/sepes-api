using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Tests.Setup
{
    public class LookupServiceMockFactory
    {
        public static ILookupService GetLookupService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<DatasetService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);
            var studyModelServiceMock = new Mock<IStudyModelService>();
            return new LookupService(mapper, userService.Object, db, studyModelServiceMock.Object);
        }
    }
}
