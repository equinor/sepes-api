using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sepes.Infrastructure.Handlers;
using Sepes.Infrastructure.Handlers.Interface;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Tests.Setup
{
    public static class StudyUpdateHandlerMockFactory   
    {  
        public static IStudyUpdateHandler Create(ServiceProvider serviceProvider, bool wbsValidationSucceeds = true)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            UserFactory.GetUserServiceForDatasetAdmin()
            var logoCreateServiceMock = new Mock<IStudyLogoCreateService>();
            var logoDeleteServiceMock = new Mock<IStudyLogoDeleteService>(); 
            
            var studyWbsValidationService = StudyServiceMockFactory.GetStudyWbsValidationServiceMock(wbsValidationSucceeds);
            var updateStudyWbsHandler = new Mock<IStudyWbsUpdateHandler>();
            var studyEfModelOperationsService = StudyEfModelOperationsServiceMockFactory.Create(serviceProvider);

            return new StudyUpdateHandler(db, logoCreateServiceMock.Object, logoDeleteServiceMock.Object , studyWbsValidationService.Object, updateStudyWbsHandler.Object, studyEfModelOperationsService);
        }     
    }
}
