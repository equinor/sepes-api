using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Tests.Setup
{
    public class StudyParticipantMockFactory
    {
        public static IStudyParticipantLookupService GetStudyParticipantLookupService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyParticipantLookupService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var azureUserService = new Mock<IAzureUserService>();

            var provisioningQueueService = new Mock<IProvisioningQueueService>();

            var cloudResourceOperationCreateService = new Mock<ICloudResourceOperationCreateService>();

            var cloudResourceOperationUpdateService = new Mock<ICloudResourceOperationUpdateService>();

            return new StudyParticipantLookupService(db, logger, mapper, userService.Object, azureUserService.Object, provisioningQueueService.Object,
                cloudResourceOperationCreateService.Object, cloudResourceOperationUpdateService.Object);
        }
    }
}
