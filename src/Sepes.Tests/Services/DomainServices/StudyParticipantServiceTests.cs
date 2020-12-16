using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Constants;
using Sepes.Tests.Setup;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services
{
    public class StudyParticipantServiceTests : ServiceTestBase
    {
        public StudyParticipantServiceTests()
            : base()
        {

        }

        [Theory]
        [InlineData(1, "Sponsor Rep", ParticipantSource.Azure)]
        [InlineData(1, "Vendor Admin", ParticipantSource.Azure)]
        [InlineData(1, "Vendor Contributor", ParticipantSource.Azure)]
        [InlineData(1, "Study Viewer", ParticipantSource.Azure)]
        [InlineData(1, "Sponsor Rep", ParticipantSource.Db)]
        [InlineData(1, "Vendor Admin", ParticipantSource.Db)]
        [InlineData(1, "Vendor Contributor", ParticipantSource.Db)]
        [InlineData(1, "Study Viewer", ParticipantSource.Db)]
        public async Task AddingParticipant_ShouldSucceed(int studyId, string role, string source)
        {
            var createTask = SetupAndAddParticipantForSet(studyId, role, source);

            await createTask;

            var createdParticipant = createTask.Result;

            Assert.NotNull(createdParticipant);
            Assert.Equal(role, createdParticipant.Role);
        }


        [Theory]
        [InlineData(1, "Sponsor R1ep", ParticipantSource.Azure)]
        [InlineData(1, "Vendor Ad1min", ParticipantSource.Azure)]
        [InlineData(1, "Vendor Contr1ibutor", ParticipantSource.Azure)]
        [InlineData(1, "Study Vie1wer", ParticipantSource.Azure)]
        [InlineData(1, "Sponsor Re1p", ParticipantSource.Db)]
        [InlineData(1, "Vendor Ad1min", ParticipantSource.Db)]
        [InlineData(1, "Vendor Contr1ibutor", ParticipantSource.Db)]
        [InlineData(1, "Study Vie1wer", ParticipantSource.Db)]
        public async Task AddingParticipant_WithNonExistingRole_ShouldThrow(int studyId, string role, string source)
        {
            var createTask = SetupAndAddParticipantForSet(studyId, role, source);

            await Assert.ThrowsAsync<ArgumentException>(() => createTask);
        }

        public async Task<StudyParticipantDto> SetupAndAddParticipantForSet(int studyId, string role, string source)
        {
            //SETUP          
            var userName = "newUserUsername";
            var userEmail = userName + "@somedomain.com";
            var userFullName = "Newly Added User";

            var participantToAdd = new ParticipantLookupDto() { DatabaseId = UserConstants.COMMON_CUR_USER_DB_ID, ObjectId = UserConstants.COMMON_CUR_USER_OBJECTID, EmailAddress = userEmail, FullName = userFullName, UserName = userName, Source = source };

            await RefreshAndPopulateTestDb();

            //GET REQUIRED SERVICES
            var db = _serviceProvider.GetService<SepesDbContext>();
            var mapper = _serviceProvider.GetService<IMapper>();

            var adUserServiceMock = new Mock<IAzureUserService>();
            adUserServiceMock.Setup(service => service.GetUserAsync(It.IsAny<string>())).ReturnsAsync(new AzureUserDto() { DisplayName = userFullName, Mail = userEmail });

            //Used to get current user
            var userServiceMock = GetUserServiceMock(UserConstants.COMMON_CUR_USER_DB_ID, UserConstants.COMMON_CUR_USER_OBJECTID);

            if (source == ParticipantSource.Db)
            {
                var studyParticipantService = new StudyParticipantService(db, mapper, userServiceMock.Object, adUserServiceMock.Object);
                return await studyParticipantService.HandleAddParticipantAsync(studyId, participantToAdd, role);
            }
            else
            {
                var studyParticipantService = new StudyParticipantService(db, mapper, userServiceMock.Object, adUserServiceMock.Object);
                return await studyParticipantService.HandleAddParticipantAsync(studyId, participantToAdd, role);
            }
        }       

        Mock<IUserService> GetUserServiceMock(int id, string objectId = UserConstants.COMMON_CUR_USER_OBJECTID)
        {
            return UserFactory.GetUserServiceMockForAdmin(id, objectId);
        }       

        async Task RefreshAndPopulateTestDb()
        {
            await ClearTestDatabase();
            var db = _serviceProvider.GetService<SepesDbContext>();

            StudyPopulator.Add(db, "Test Study 1", "Vendor for TS1", "WBS for TS1", UserConstants.COMMON_CUR_USER_DB_ID);

            StudyPopulator.Add(db, "Test Study 2", "Vendor for TS2", "WBS for TS2", UserConstants.COMMON_CUR_USER_DB_ID);

            StudyPopulator.Add(db, "Test Study 3", "Vendor for TS3", "WBS for TS3", UserConstants.COMMON_CUR_USER_DB_ID);

            db.SaveChanges();
        }
    }
}
