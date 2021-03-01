using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class MultipleControllerReadTests : ControllerTestBase
    {
        public MultipleControllerReadTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        //Can be used to read study, sandbox, vm, dataset

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task Read_AnyStudyRelatedEntity_AsAdmin_ShouldSucceed(bool createdByCurrentUser, bool restrictedStudy)
        {            
            SetScenario(isAdmin: true);
            await WithUserSeeds();
            var sandbox = await WithSandbox(createdByCurrentUser, restrictedStudy);
            await ReadAllAndAssertExpectSuccess(sandbox);
        }

        [Theory]
        [InlineData( false)]
        [InlineData( true)]    
        public async Task Read_OwnedStudyRelatedEntity_AsSponsor_ShouldSucceed(bool restrictedStudy)
        {
            SetScenario(isSponsor: true);
            await WithUserSeeds();
            var sandbox = await WithSandbox(true, restrictedStudy);
            await ReadAllAndAssertExpectSuccess(sandbox);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(false, StudyRoles.SponsorRep)]
        [InlineData(false, StudyRoles.VendorAdmin)]
        [InlineData(false, StudyRoles.VendorContributor)]
        [InlineData(false, StudyRoles.StudyViewer)]
        [InlineData(true, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.VendorAdmin)]
        [InlineData(true, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer)]

        public async Task Read_AnyStudyRelatedEntity_WithCorrectStudyRole_ShouldSucceed(bool restrictedStudy, string studyRole = null)
        {
            SetScenario(isEmployee: true);
            await WithUserSeeds();
            var sandbox = await WithSandbox(false, restrictedStudy, studyRole);
            await ReadAllAndAssertExpectSuccess(sandbox);                  
        }     


        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]       
        public async Task Read_RestrictedStudy_WithoutRelevantRoles_ShouldFail(bool employee, bool sponsor, bool datasetAdmin)
        {
            SetScenario(isEmployee: employee, isSponsor: sponsor, isDatasetAdmin: datasetAdmin);
            await WithUserSeeds();

            var sandbox = await WithSandbox(false, true);

            await ReadAllAndAssertExpectForbidden(sandbox);
        }

        async Task ReadAllAndAssertExpectSuccess(Sandbox sandbox)
        {
            await GenericReader.ReadAndAssertExpectSuccess<StudyDetailsDto>(_restHelper, GenericReader.StudyUrl(sandbox.StudyId));
            await GenericReader.ReadAndAssertExpectSuccess<SandboxDetails>(_restHelper, GenericReader.SandboxUrl(sandbox.Id));
        }

        async Task ReadAllAndAssertExpectForbidden(Sandbox sandbox) {
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.StudyUrl(sandbox.StudyId));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxUrl(sandbox.Id));
        }       
    }
}
