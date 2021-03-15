using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyControllerTests : ControllerTestBase
    {
        public StudyControllerTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task AddStudy_WithoutVendor_ShouldFail(bool isAdmin, bool isSponsor)
        {
            SetScenario(isEmployee: true, isAdmin, isSponsor);

            var responseWrapper = await StudyCreator.CreateAndExpectFailure(_restHelper, vendor: null);
            CreateStudyAsserts.ExpectValidationFailure(responseWrapper.Response, "The Vendor field is required");
       
        }

        [Theory]
        [InlineData(true, false)]
        public async Task AddStudy_ShouldCreateResourceGroupForStudySpecificDatasets(bool isAdmin, bool isSponsor)
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
    }
}
