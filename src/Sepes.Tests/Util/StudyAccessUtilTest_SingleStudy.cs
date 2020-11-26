using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
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

            var returnedStudy = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userSerice, COMMON_STUDY_ID, UserOperation.Study_Read);
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
            var study = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userSerice, COMMON_STUDY_ID, UserOperation.Study_Read);

            Assert.NotNull(study);
            Assert.Equal(COMMON_STUDY_ID, study.Id);

            Assert.NotNull(study.StudyParticipants);
            Assert.NotEmpty(study.StudyParticipants);

            var studyParticipant = study.StudyParticipants.FirstOrDefault();
            Assert.NotNull(studyParticipant);
            Assert.NotNull(studyParticipant.User);
            Assert.Equal(COMMON_USER_ID, studyParticipant.User.Id);
       
        }

        //TODO: 20201126: KRST: NEED TO INTRODUCE MOQ TO FAKE THE USER SERVICE TO BE ABLE TO TEST THIS
        //[Theory]        
        //[InlineData("null")]
        //[InlineData("Not a real role")]
        //[InlineData("")]
        //public async void ReadingRestrictedStudyWithoutPermission_ShouldFail(string justSomeBogusRole)
        //{
        //    var db = GetContextWithSimpleTestData(COMMON_USER_ID, COMMON_STUDY_ID, true, justSomeBogusRole);
            
        //    var userSerice = ServiceProvider.GetService<IUserService>();
        //    await Assert.ThrowsAsync<ForbiddenException>(()=> StudySingularQueries.GetStudyByIdCheckAccessOrThrow(db, userSerice, COMMON_STUDY_ID, justSomeBogusRole));     
        //}  
     
        
    }
}
