using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Tests.Setup;
using System.Linq;
using Xunit;

namespace Sepes.Tests.Util
{
    public class StudyAccessUtilTest_SingleStudy : StudyAccessUtilTest_Base
    {
        public StudyAccessUtilTest_SingleStudy()
            : base()
        {
        
        }

        [Fact]
        public async void ReadingUnrestrictedStudy_ShouldBeAllowed()
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, false);
            
            var userServiceMock = UserFactory.GetUserServiceMockForBasicUser();        

            var returnedStudy = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_Read);
            Assert.NotNull(returnedStudy); 
        }

        //I am owner, but not admin
        //I am owner

   

     

        [Fact]  
        public async void ReadingRestrictedStudy_AsAdmin_ShouldSucceeed()
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, true);

            var userServiceMock = UserFactory.GetUserServiceMockForAdmin(1);

            var study = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_Read);

            PerformUsualStudyTests(study);
        }

        [Theory]
        [InlineData(AppRoles.Admin, StudyRoles.StudyOwner)]
        [InlineData(AppRoles.Sponsor, StudyRoles.StudyOwner)]
        public async void ReadingRestrictedStudy_WithRelevantAppRole_ShouldSucceeed(string appRole, string studySpecificRole)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, true, studySpecificRole);

            var userServiceMock = UserFactory.GetUserServiceMockForAppRole(appRole, 1);
       
            var study = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_Read);

            PerformUsualStudyTests(study);
            UserMustBeAmongStudyParticipants(study);
        }

        [Theory]
        [InlineData(AppRoles.DatasetAdmin, StudyRoles.StudyOwner)]
        public async void ReadingRestrictedStudy_WithWrongAppRole_ShouldFail(string appRole, string studySpecificRole)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, true, studySpecificRole);

            var userServiceMock = UserFactory.GetUserServiceMockForAppRole(appRole, 1);

            await Assert.ThrowsAsync<ForbiddenException>(() => StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_Read));
        }

        [Theory] 
        [InlineData(StudyRoles.SponsorRep)]
        [InlineData(StudyRoles.VendorAdmin)]
        [InlineData(StudyRoles.VendorContributor)]
        [InlineData(StudyRoles.StudyViewer)]
        public async void ReadingRestrictedStudyWithRelevantPermission_ShouldSucceed(string studySpecificRole)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, true, studySpecificRole);

            var userServiceMock = UserFactory.GetUserServiceMockForBasicUser(1);
            var study = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_Read);

            PerformUsualStudyTests(study);
            UserMustBeAmongStudyParticipants(study);
        }   


        [Theory]
        [InlineData("null")]
        [InlineData("Not a real role")]
        [InlineData("")]
        [InlineData(StudyRoles.StudyOwner)]
        public async void ReadingRestrictedStudyThatHasBogusRole_ShouldFail(string justSomeBogusRole)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, true, justSomeBogusRole);

            var userServiceMock = UserFactory.GetUserServiceMockForBasicUser(1);
            await Assert.ThrowsAsync<ForbiddenException>(() => StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_Read));
        }

        void PerformUsualStudyTests(Study study)
        {
            Assert.NotNull(study);
            Assert.Equal(COMMON_STUDY_ID, study.Id);

            Assert.NotNull(study.StudyParticipants);       
        }

        void UserMustBeAmongStudyParticipants(Study study)
        {         
            Assert.NotEmpty(study.StudyParticipants);

            var studyParticipant = study.StudyParticipants.FirstOrDefault();
            Assert.NotNull(studyParticipant);
            Assert.NotNull(studyParticipant.User);
            Assert.Equal(COMMON_USER_ID, studyParticipant.User.Id);
        }
    }
}
