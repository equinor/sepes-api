using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Common.Constants;
using System;

namespace Sepes.Tests.Setup
{
    public class StudyParticipantRolesServiceMockFactory
    {
        public static IStudyParticipantRolesService GetForBasicUser(ServiceProvider serviceProvider)
        {
            var userService = UserFactory.GetUserServiceMockForBasicUser(true, UserTestConstants.COMMON_CUR_USER_DB_ID);
            return GetInternal(serviceProvider, userService);
        }

        public static IStudyParticipantRolesService GetForAdmin(ServiceProvider serviceProvider)
        {
            var userService = UserFactory.GetUserServiceMockForAdmin(UserTestConstants.COMMON_CUR_USER_DB_ID);
            return GetInternal(serviceProvider, userService);
        }

        public static IStudyParticipantRolesService GetForSponsor(ServiceProvider serviceProvider)
        {
            var userService = UserFactory.GetUserServiceMockForSponsor(UserTestConstants.COMMON_CUR_USER_DB_ID);
            return GetInternal(serviceProvider, userService);
        }

        public static IStudyParticipantRolesService GetForDatasetAdmin(ServiceProvider serviceProvider)
        {
            var userService = UserFactory.GetUserServiceMockForDatasetAdmin(UserTestConstants.COMMON_CUR_USER_DB_ID);
            return GetInternal(serviceProvider, userService);
        }

        public static IStudyParticipantRolesService GetForEmployee(ServiceProvider serviceProvider)
        {
            var userService = UserFactory.GetUserServiceMockForBasicUser(true, UserTestConstants.COMMON_CUR_USER_DB_ID);
            return GetInternal(serviceProvider, userService);
        }

        static IStudyParticipantRolesService GetInternal(ServiceProvider serviceProvider, Mock<IUserService> userService)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var mapper = serviceProvider.GetService<IMapper>();
           
            var studyModelServiceMock = StudyModelServiceMockFactory.StudyEfModelService(serviceProvider);
            return new StudyParticipantRolesService(db, mapper, userService.Object, studyModelServiceMock);
        }

      
    }
}
