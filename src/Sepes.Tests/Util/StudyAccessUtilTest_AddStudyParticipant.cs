using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Util
{
    public class StudyAccessUtilTest_AddStudyParticipant : StudyAccessUtilTest_Base
    {
        public StudyAccessUtilTest_AddStudyParticipant()
            : base()
        {
        
        }

        [Theory]
        [InlineData(false, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(false, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async void AddingStudyParticipantWithoutRelevantRoles_ShouldThrow(bool restrictedStudy, bool employees, params string[] rolesToAdd)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, restrictedStudy);
            
            var userServiceMock = UserFactory.GetUserServiceMockForBasicUser(employees, COMMON_USER_ID);

            foreach(var curRole in rolesToAdd)
            {
                await Assert.ThrowsAsync<ForbiddenException>(() => StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_AddRemove_Participant, true, curRole));
            }        
        }

        [Theory]
        [InlineData(false, AppRoles.Admin, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, AppRoles.Admin, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(false, AppRoles.Sponsor, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, AppRoles.Sponsor, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async void AddingStudyParticipant_HavingRelevantAppRole_AndAsStudyOwner_ShouldSucceeed(bool restrictedStudy, string appRoleForUser, params string[] rolesToAdd)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, restrictedStudy, StudyRoles.StudyOwner);

            var userServiceMock = UserFactory.GetUserServiceMockForAppRole(appRoleForUser, COMMON_USER_ID);

            foreach (var curRole in rolesToAdd)
            {
                var study = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_AddRemove_Participant, true, curRole);

                PerformUsualStudyTests(study);
                UserMustBeAmongStudyParticipants(study);
            }
        }

        [Theory]
        [InlineData(false, AppRoles.DatasetAdmin, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, AppRoles.DatasetAdmin, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async void AddingStudyParticipant_MissingRelevantAppRole_AsStudyOwner_ShouldThrow(bool restrictedStudy, string appRoleForUser, params string[] rolesToAdd)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, restrictedStudy, StudyRoles.StudyOwner);

            var userServiceMock = UserFactory.GetUserServiceMockForAppRole(appRoleForUser, COMMON_USER_ID);
            foreach (var curRole in rolesToAdd)
            {
                await Assert.ThrowsAsync<ForbiddenException>(() => StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_AddRemove_Participant, true, curRole));
            }
        }

        [Theory]
        [InlineData(false, false, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(false, true, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, false, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, true, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async void VendorAdmin_AddingVendorRoles_ShouldSucceed(bool employee, bool restrictedStudy, params string[] rolesToAdd)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, restrictedStudy, StudyRoles.VendorAdmin);

            var userServiceMock = UserFactory.GetUserServiceMockForUserWithStudyRole(employee, COMMON_USER_ID);

            foreach (var curRole in rolesToAdd)
            {
                var study = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_AddRemove_Participant, true, curRole);

                PerformUsualStudyTests(study);
                UserMustBeAmongStudyParticipants(study);
            }
        }

        [Theory]
        [InlineData(false, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep)]
        [InlineData(false, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep)]
        [InlineData(true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep)]
        [InlineData(true, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep)]
        public async void VendorAdmin_AddingNonVendorRoles_ShouldThrow(bool employee, bool restrictedStudy, params string[] rolesToAdd)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, restrictedStudy, StudyRoles.VendorAdmin);

            var userServiceMock = UserFactory.GetUserServiceMockForUserWithStudyRole(employee, COMMON_USER_ID);

            foreach (var curRole in rolesToAdd)
            {
                await Assert.ThrowsAsync<ForbiddenException>(() => StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userServiceMock.Object, COMMON_STUDY_ID, UserOperation.Study_AddRemove_Participant, true, curRole));
            }
        }
    }
}
