using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Tests.Setup
{
    public class LookupServiceMockFactory
    {
        public static ILookupService GetLookupService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();          
            var userService = UserFactory.GetUserServiceMockForAdmin(1);
            var studyModelServiceMock = StudyServiceMockFactory.StudyEfModelService(serviceProvider);
            return new LookupService(db, mapper, userService.Object, studyModelServiceMock);
        }
    }
}
