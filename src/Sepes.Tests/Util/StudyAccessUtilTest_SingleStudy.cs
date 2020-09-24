using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
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
          
            var userSerice = ServiceProvider.GetService<IUserService>();

            var returnedStudy = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(db, userSerice, COMMON_STUDY_ID, UserOperations.StudyReadOwnRestricted);
            Assert.NotNull(returnedStudy); 
        }

        [Theory]
        [InlineData(StudyRoles.StudyOwner)]
        [InlineData(StudyRoles.SponsorRep)]
        [InlineData(StudyRoles.VendorAdmin)]
        [InlineData(StudyRoles.VendorContributor)]
        [InlineData(StudyRoles.StudyViewer)]
        public async void ReadingRestrictedStudyWithPermission_ShouldSucceed(string roleThatGrantsPermission)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, true, roleThatGrantsPermission);

            var userSerice = ServiceProvider.GetService<IUserService>();
            var study = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(db, userSerice, COMMON_STUDY_ID, UserOperations.StudyReadOwnRestricted);

            Assert.NotNull(study);
            Assert.Equal(COMMON_STUDY_ID, study.Id);

            Assert.NotNull(study.StudyParticipants);
            Assert.NotEmpty(study.StudyParticipants);

            var studyParticipant = study.StudyParticipants.FirstOrDefault();
            Assert.NotNull(studyParticipant);
            Assert.NotNull(studyParticipant.User);
            Assert.Equal(COMMON_USER_ID, studyParticipant.User.Id);
       
        }

        [Theory]        
        [InlineData("null")]
        [InlineData("Not a real role")]
        [InlineData("")]
        [InlineData(AppRoles.Admin)] //Not a study specific role, so cannot be used this way
        [InlineData(AppRoles.Sponsor)] //Not a study specific role, so cannot be used this way
        [InlineData(AppRoles.DatasetAdmin)] //Not a study specific role, so cannot be used this way
        public async void ReadingRestrictedStudyWithoutPermission_ShouldFail(string justSomeBogusRole)
        {
            var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, true, justSomeBogusRole);
            
            var userSerice = ServiceProvider.GetService<IUserService>();
            await Assert.ThrowsAsync<ForbiddenException>(()=> StudyAccessUtil.GetStudyAndCheckAccessOrThrow(db, userSerice, COMMON_STUDY_ID, UserOperations.StudyReadOwnRestricted));     
        }  
     
        
    }
}
