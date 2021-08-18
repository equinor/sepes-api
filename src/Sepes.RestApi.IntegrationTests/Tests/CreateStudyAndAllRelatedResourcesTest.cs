using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Common.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Dataset;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.StudyParticipant;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class CreateStudyAndAllRelatedResourcesTest : ControllerTestBase
    {
        public CreateStudyAndAllRelatedResourcesTest(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task AddStudyAndSandboxAndVm_WithRequiredRole_ShouldSucceed(bool isAdmin, bool isSponsor)
        {
            Trace.WriteLine("START AddStudyAndSandboxAndVm_WithRequiredRole_ShouldSucceed");
            await WithBasicSeeds();

            SetScenario(isEmployee: true, isAdmin: isAdmin, isSponsor: isSponsor);

            //CREATE STUDY
            var studyCreateConversation = await StudyCreator.CreateAndExpectSuccess(_restHelper);
            CreateStudyAsserts.ExpectSuccess(studyCreateConversation.Request, studyCreateConversation.Response);

            //CREATE STUDY SPECIFIC DATASET
            var datasetSeedResponse = await StudySpecificDatasetCreateUpdateDelete.CreateExpectSuccess(_restHelper, studyCreateConversation.Response.Content.Id);
            var datasetCreateRequest = datasetSeedResponse.Request;
            var datasetResponseWrapper = datasetSeedResponse.Response;
            CreateDatasetAsserts.ExpectSuccess(datasetCreateRequest, datasetResponseWrapper);

            var createDatasetResponse = datasetResponseWrapper.Content;

            //CREATE SANDBOX
            var sandboxSeedResponse = await SandboxCreator.CreateAndExpectSuccess(_restHelper, studyCreateConversation.Response.Content.Id);
            var sandboxCreateRequest = sandboxSeedResponse.Request;
            var sandboxResponseWrapper = sandboxSeedResponse.Response;

            SandboxDetailsAsserts.NewlyCreatedExpectSuccess(sandboxCreateRequest, sandboxResponseWrapper);

            var sandboxResponse = sandboxResponseWrapper.Content;

            //ADD DATASET TO SANDBOX
            var addDatasetToSandboxResponse = await SandboxDatasetOperations.AddDatasetExpectSuccess(_restHelper, sandboxResponse.Id, createDatasetResponse.Id);
            var sandboxDatasetResponseWrapper = addDatasetToSandboxResponse.Response;
            AddDatasetToSandboxAsserts.ExpectSuccess(createDatasetResponse.Id, createDatasetResponse.Name, createDatasetResponse.Classification, "Open", sandboxDatasetResponseWrapper);

            //CREATE VM
            var virtualMachineSeedResponse = await VirtualMachineCreator.CreateAndExpectSuccess(_restHelper, sandboxResponse.Id);
            var virtualMachineCreateRequest = virtualMachineSeedResponse.Request;
            var virtualMachineResponseWrapper = virtualMachineSeedResponse.Response;

            CreateVirtualMachineAsserts.ExpectSuccess(virtualMachineCreateRequest, sandboxResponse.Region, virtualMachineResponseWrapper);

            //GET SANDBOX RESOURCE LIST AND ASSERT RESULT BEFORE CREATION
            var sandboxResourcesPreProvisioningResponseWrapper = await _restHelper.Get<List<SandboxResourceLight>>($"api/sandboxes/{sandboxResponse.Id}/resources");
            SandboxResourceListAsserts.BeforeProvisioning(sandboxResourcesPreProvisioningResponseWrapper, virtualMachineResponseWrapper.Content.Name);

            //GET SANDBOX VM LIST AND ASSERT RESULT BEFORE CREATION
            var virtualMachinesPreProvisioningResponseWrapper = await GenericReader.ReadAndAssertExpectSuccess<List<VmDto>>(_restHelper, GenericReader.SandboxVirtualMachinesUrl(sandboxResponse.Id));
            SandboxVirtualMachineAsserts.BeforeProvisioning(virtualMachinesPreProvisioningResponseWrapper.Response, virtualMachineResponseWrapper.Content.Name);

            var vmInfoExtended = await _restHelper.Get<VmExtendedDto>($"api/virtualmachines/{virtualMachineResponseWrapper.Content.Id}/extended");
            VirtualMachineExtendedInfoAsserts.BeforeProvisioningExpectSuccess(vmInfoExtended);

            //SETUP INFRASTRUCTURE BY RUNNING A METHOD ON THE API            
            var processWorkQueueResponse = await ProcessWorkQueue();

            //GET SANDBOX RESOURCE LIST AND ASSERT
            var sandboxResourcesResponseWrapper = await _restHelper.Get<List<SandboxResourceLight>>($"api/sandboxes/{sandboxResponse.Id}/resources");
            SandboxResourceListAsserts.AfterProvisioning(sandboxResourcesResponseWrapper, virtualMachineResponseWrapper.Content.Name);

            //GET SANDBOX VM LIST AND ASSERT
            var virtualMachinesAfterProvisioningResponseWrapper = await GenericReader.ReadAndAssertExpectSuccess<List<VmDto>>(_restHelper, GenericReader.SandboxVirtualMachinesUrl(sandboxResponse.Id));
            SandboxVirtualMachineAsserts.AfterProvisioning(virtualMachinesAfterProvisioningResponseWrapper.Response, virtualMachineResponseWrapper.Content.Name);

            var vmInfoExtendedAfter = await _restHelper.Get<VmExtendedDto>($"api/virtualmachines/{virtualMachineResponseWrapper.Content.Id}/extended");
            VirtualMachineExtendedInfoAsserts.AfterProvisioningExpectSuccess(vmInfoExtendedAfter);

            //Add some participants

            var studyParticipantResponse = await StudyParticipantAdderAndRemover.AddAndExpectSuccess(_restHelper, studyCreateConversation.Response.Content.Id, StudyRoles.SponsorRep,
                StudyParticipantAdderAndRemover.CreateParticipantLookupDto());

            var getStudy = await _restHelper.Get<StudyDetailsDto>($"api/studies/{studyCreateConversation.Response.Content.Id}");

            var studyParticipant = getStudy.Content.Participants.Find(x => x.UserId == studyParticipantResponse.Response.Content.UserId);

            AddStudyParticipantsAsserts.ExpectSuccess(StudyRoles.SponsorRep, studyParticipant, studyParticipantResponse.Response);

            //Get rules
            var vmRules = await GenericReader.ReadAndAssertExpectSuccess<List<VmRuleDto>>(_restHelper, GenericReader.VirtualMachineRulesUrl(virtualMachineResponseWrapper.Content.Id));

            //OPEN INTERNET
            var openInternetResponse = await SandboxOperations.OpenInternetForVmExpectSuccess(_restHelper, virtualMachineResponseWrapper.Content.Id, vmRules.Response.Content);

            SandboxVirtualMachineRuleAsserts.ExpectSuccess(openInternetResponse);
                      
            _ = await ProcessWorkQueue();

            var closeInternetResponse = await SandboxOperations.CloseInternetForVmExpectSuccess(_restHelper, virtualMachineResponseWrapper.Content.Id, vmRules.Response.Content);

            SandboxVirtualMachineRuleAsserts.ExpectSuccess(closeInternetResponse);

            _ = await ProcessWorkQueue();

            //MOVE TO NEXT PHASE
            var sandboxAfterMovingToNextPhase = await SandboxOperations.MoveToNextPhase<SandboxDetails>(_restHelper, sandboxResponseWrapper.Content.Id);

            SandboxDetailsAsserts.AfterPhaseShiftExpectSuccess(sandboxAfterMovingToNextPhase.Response);

            //DELETE VM
            var deleteVmConversation = await SandboxOperations.DeleteVm(_restHelper, virtualMachineResponseWrapper.Content.Id);
            ApiResponseBasicAsserts.ExpectNoContent(deleteVmConversation.Response);

            //RUN WORKER
            await ProcessWorkQueue();

            //ASSERT THAT VM DISSAPEARS
            var sandboxVmsAfterDelete = await _restHelper.Get<List<VmDto>>($"api/virtualmachines/forsandbox/{sandboxResponseWrapper.Content.Id}");

            SandboxVirtualMachineAsserts.AfterProvisioning(sandboxVmsAfterDelete, "vm-studyname-sandboxnam-integrationtest");

            //TRY TO DELETE STUDY, GET ERROR
            await StudyDeleter.DeleteAndExpectFailure(_restHelper, studyCreateConversation.Response.Content.Id);

            //DELETE SANDBOX
            await SandboxDeleter.DeleteAndExpectSuccess(_restHelper, sandboxResponseWrapper.Content.Id);

            //DELETE STUDY
            await StudyDeleter.DeleteAndExpectSuccess(_restHelper, studyCreateConversation.Response.Content.Id);
            Trace.WriteLine("START AddStudyAndSandboxAndVm_WithRequiredRole_ShouldSucceed");
        }
    }
}
