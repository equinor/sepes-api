//using Sepes.Infrastructure.Constants;
//using Sepes.RestApi.IntegrationTests.RequestHelpers;
//using Sepes.RestApi.IntegrationTests.Setup;
//using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
//using System.Threading.Tasks;
//using Xunit;

//namespace Sepes.RestApi.IntegrationTests.Tests
//{
//    [Collection("Integration tests collection")]
//    public class StudyControllerReadTests : ControllerTestBase
//    {
//        public StudyControllerReadTests(TestHostFixture testHostFixture)
//            : base(testHostFixture)
//        {

//        }

//        [Theory]
//        [InlineData(false, false, false, true)]
//        [InlineData(false, false, true, false)]
//        [InlineData(false, true, false, false)]
//        [InlineData(true, false, false, true)]
//        [InlineData(true, false, true, false)]         
//        public async Task Read_Study_CreatedByMe_ShouldSucceed(bool restrictedStudy, bool isEmployee, bool isAdmin, bool isSponsor)
//        {
//            SetScenario(isEmployee: isEmployee, isAdmin: isAdmin, isSponsor: isSponsor);
//            await WithBasicSeeds();
//            var createdStudy = await WithStudyCreatedByCurrentUser(restricted: restrictedStudy);

//            var studyCreateConversation = await StudyReader.ReadAndExpectSuccess(_restHelper, createdStudy.Id);

//            ReadStudyAsserts.ExpectSuccess(studyCreateConversation.Response);
//        }

//        [Theory]
//        [InlineData(false, true, false, false, null)]
//        [InlineData(false, false, true, false, null)]

//        //Sponsor needs relevant role
//        [InlineData(false, false, false, true, StudyRoles.SponsorRep)]
//        [InlineData(false, false, false, true, StudyRoles.VendorAdmin)]
//        [InlineData(false, false, false, true, StudyRoles.VendorContributor)]
//        [InlineData(false, false, false, true, StudyRoles.StudyViewer)]

//        [InlineData(true, false, true, false, null)]
//        [InlineData(true, true, false, true, StudyRoles.SponsorRep)]
//        [InlineData(true, true, false, true, StudyRoles.VendorAdmin)]
//        [InlineData(true, true, false, true, StudyRoles.VendorContributor)]
//        [InlineData(true, true, false, true, StudyRoles.StudyViewer)]    
//        public async Task Read_Study_CreatedByOther_ShouldSucceed(bool restrictedStudy, bool employee, bool isAdmin, bool isSponsor, string roleName)
//        {
//            SetScenario(isEmployee: employee, isAdmin: isAdmin, isSponsor: isSponsor);
//            await WithBasicSeeds();
//            var createdStudy = await WithStudyCreatedByOtherUser(restricted: restrictedStudy, roleName);

//            var studyCreateConversation = await StudyReader.ReadAndExpectSuccess(_restHelper, createdStudy.Id);

//            ReadStudyAsserts.ExpectSuccess(studyCreateConversation.Response);
//        }


//        //[Theory]
//        //[InlineData(true, false)]
//        //[InlineData(false, true)]
//        //[InlineData(true, true)]
//        //public async Task Read_Study_CreatedByOther_WithoutRelevantRoles_ShouldFail(bool restrictedStudy, bool employee, bool isAdmin, bool isSponsor, string roleName)
//        //{
//        //    SetScenario(isEmployee: isEmployee, isDatasetAdmin: isDatasetAdmin);

//        //    var studyCreateConversation = await StudyCreator.CreateAndExpectFailure(_restHelper);

//        //     ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyCreateConversation.Response, "does not have permission to perform operation");
//        //}

      
//    }
//}
