using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Threading.Tasks;
using Xunit;
using Sepes.RestApi.IntegrationTests.TestHelpers;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyCreateControllerShould : ControllerTestBase
    {
        public StudyCreateControllerShould(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }


        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task Fail_If_Study_MissingVendor(bool isAdmin, bool isSponsor)
        {
            SetScenario(isEmployee: true, isAdmin, isSponsor);

            var responseWrapper = await StudyCreator.CreateAndExpectFailure(_restHelper, vendor: null);
            CreateStudyAsserts.ExpectValidationFailure(responseWrapper.Response, "The Vendor field is required");

        }

        [Theory]
        [InlineData(true, false)]
        public async Task Create_ResourceGroupForStudySpecificDatasets(bool isAdmin, bool isSponsor)
        {
            SetScenario(isEmployee: true, isAdmin: isAdmin, isSponsor: isSponsor);

            var createStudyApiConversation = await StudyCreator.CreateAndExpectSuccess(_restHelper);

            CreateStudyAsserts.ExpectSuccess(createStudyApiConversation.Request, createStudyApiConversation.Response);

            var databaseEntryForStudyDatasetResourceGroup = await SliceFixture.GetResource(studyId: createStudyApiConversation.Response.Content.Id);
            CloudResourceBasicAsserts.StudyDatasetResourceGroupBeforeProvisioningAssert(databaseEntryForStudyDatasetResourceGroup);

            //SETUP INFRASTRUCTURE BY RUNNING A METHOD ON THE API            
            _ = await ProcessWorkQueue();

            //Get resource from database again and assert
            databaseEntryForStudyDatasetResourceGroup = await SliceFixture.GetResource(studyId: createStudyApiConversation.Response.Content.Id);
            CloudResourceBasicAsserts.StudyDatasetResourceGroupAfterProvisioningAssert(databaseEntryForStudyDatasetResourceGroup);
        }

        [Theory]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        public async Task AddStudy_IfUserHasRequiredRole(bool isEmployee, bool isAdmin, bool isSponsor)
        {
            SetScenario(isEmployee: isEmployee, isAdmin: isAdmin, isSponsor: isSponsor);

            var studyCreateConversation = await StudyCreator.CreateAndExpectSuccess(_restHelper);

            CreateStudyAsserts.ExpectSuccess(studyCreateConversation.Request, studyCreateConversation.Response);
        }


        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task Throw_IfUserLacksRequiredRole(bool isEmployee, bool isDatasetAdmin)
        {
            SetScenario(isEmployee: isEmployee, isDatasetAdmin: isDatasetAdmin);

            var studyCreateConversation = await StudyCreator.CreateAndExpectFailure(_restHelper);

            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyCreateConversation.Response, "does not have permission to perform operation");
        }       
    }
}
