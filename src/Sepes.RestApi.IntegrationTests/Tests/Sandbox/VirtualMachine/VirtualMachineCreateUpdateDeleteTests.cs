using Sepes.Common.Constants;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class VirtualMachineCreateUpdateDeleteTests : ControllerTestBase
    {
        public VirtualMachineCreateUpdateDeleteTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {
           
        }       

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task Admin_CanPerformAllOperations(bool studyCreatedByCurrentUser, bool restrictedStudy)
        {
            await WithBasicSeeds();
            SetScenario(isAdmin: true);               

            await PerformTestsExpectSuccess(studyCreatedByCurrentUser, restrictedStudy);         
        }

        [Theory]       
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddAndRemove_ToOwnedStudy_AsSponsor_ShouldSucceed(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            await PerformTestsExpectSuccess(true, restrictedStudy);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddAndRemove_ToNonOwnedStudy_AsSponsor_ShouldFail(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            await PerformTestsExpectFailure(false, restrictedStudy);
        }

        [Theory]
        [InlineData(false, StudyRoles.SponsorRep)]
        [InlineData(false, StudyRoles.VendorAdmin)]
        [InlineData(true, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.VendorAdmin)]      
        public async Task AddAndRemove_HavingCorrectStudyRoles_ShouldSucceed(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            await PerformTestsExpectSuccess(false, restrictedStudy, studyRole);
        }

        [Theory]
        [InlineData(false, StudyRoles.VendorContributor)]
        [InlineData(false, StudyRoles.StudyViewer)] 
        [InlineData(true, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer)]
        public async Task AddAndRemove_HavingWrongStudyRoles_ShouldFail(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            await PerformTestsExpectFailure(false, restrictedStudy, studyRole);
        }

        async Task PerformTestsExpectSuccess(bool studyCreatedByCurrentUser, bool restrictedStudy, string studyRole = null)
        {
            var sandbox = await CreateSandbox(studyCreatedByCurrentUser, restrictedStudy, studyRole);

            //Create vm
            var vmCreateConversation = await VirtualMachineCreator.CreateAndExpectSuccess(_restHelper, sandbox.Id);
            SandboxVirtualMachineAsserts.ExpectSuccess(vmCreateConversation.Response);

            //Create VM manually
            var vmThatExists = await WithVm(studyCreatedByCurrentUser, restrictedStudy, studyRole);

            //Get VM Rules
            var vmRules = await GenericReader.ReadAndAssertExpectSuccess<List<VmRuleDto>>(_restHelper, GenericReader.VirtualMachineRulesUrl(vmThatExists.Id));
                  
            //Open internet        
            var openInternetResponse = await SandboxOperations.OpenInternetForVmExpectSuccess(_restHelper, vmThatExists.Id, vmRules.Response.Content);
            SandboxVirtualMachineRuleAsserts.ExpectSuccess(openInternetResponse);

            //Close internet
            var closeInternetResponse = await SandboxOperations.CloseInternetForVmExpectSuccess(_restHelper, vmThatExists.Id, vmRules.Response.Content);
            SandboxVirtualMachineRuleAsserts.ExpectSuccess(closeInternetResponse);

            //Set inbound rule
            var inboundRuleResponse = await SandboxOperations.AddVmInboundRuleExpectSuccess(_restHelper, vmThatExists.Id, vmRules.Response.Content);
            SandboxVirtualMachineRuleAsserts.ExpectSuccess(inboundRuleResponse);       

            //Delete vm
            var deleteRequest = await GenericDeleter.DeleteAndExpectSuccess(_restHelper, $"api/virtualmachines/{vmThatExists.Id}");
            ApiResponseBasicAsserts.ExpectNoContent(deleteRequest.Response);
        }      

        async Task PerformTestsExpectFailure(bool studyCreatedByCurrentUser, bool restrictedStudy, string studyRole = null)
        {
            var sandbox = await CreateSandbox(studyCreatedByCurrentUser, restrictedStudy, studyRole);

            //Create vm
            var vmCreateConversation = await VirtualMachineCreator.CreateAndExpectFailure(_restHelper, sandbox.Id);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(vmCreateConversation.Response);

            //Create VM manually
            var vmThatExists = await WithVm(studyCreatedByCurrentUser, restrictedStudy, studyRole);

            //Get VM Rules
            var existingVmRules = VmRuleUtils.CreateInitialVmRules(vmThatExists.Id);

            //Open internet        
            var openInternetResponse = await SandboxOperations.OpenInternetForVmExpectFailure(_restHelper, vmThatExists.Id, existingVmRules);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(openInternetResponse.Response);

            //Close internet
            var closeInternetResponse = await SandboxOperations.CloseInternetForVmExpectFailure(_restHelper, vmThatExists.Id, existingVmRules);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(closeInternetResponse.Response);

            //Set inbound rule
            var inboundRuleResponse = await SandboxOperations.AddVmInboundRuleExpectFailure(_restHelper, vmThatExists.Id, existingVmRules);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(inboundRuleResponse.Response);

            //Delete vm
            var deleteRequest = await GenericDeleter.DeleteAndExpectFailure(_restHelper, $"api/virtualmachines/{vmThatExists.Id}");
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(deleteRequest.Response);
        }

        async Task<Sandbox> CreateSandbox(bool studyCreatedByCurrentUser, bool restrictedStudy, string studyRole = null)
        {
            return await WithSandbox(studyCreatedByCurrentUser, restrictedStudy, string.IsNullOrWhiteSpace(studyRole) ? null : new List<string> { studyRole }, addDatasetsToStudy: true, addDatasetsToSandbox: true);
        }

        async Task<CloudResource> WithVm(bool studyCreatedByCurrentUser, bool restrictedStudy, string studyRole = null)
        {
            return await WithVirtualMachine(studyCreatedByCurrentUser, restrictedStudy, string.IsNullOrWhiteSpace(studyRole) ? null : new List<string> { studyRole }, addDatasetsToStudy: true, addDatasetsToSandbox: true);
        }
    }
}
