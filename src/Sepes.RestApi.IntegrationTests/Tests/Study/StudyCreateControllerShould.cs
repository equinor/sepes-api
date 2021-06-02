using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Threading.Tasks;
using Xunit;

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
