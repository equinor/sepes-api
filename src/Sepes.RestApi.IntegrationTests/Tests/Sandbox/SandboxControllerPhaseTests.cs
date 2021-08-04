﻿using Sepes.Common.Constants;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class SandboxControllerPhaseTests : ControllerTestBase
    {
        public SandboxControllerPhaseTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {
           
        }    

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task IncreasePhase_AsAdmin_ShouldSucceed(bool studyCreatedByCurrentUser, bool restrictedStudy)
        {
            await WithBasicSeeds();
            SetScenario(isAdmin: true);           

            var virtualMachine = await WithVirtualMachine(studyCreatedByCurrentUser, restrictedStudy);

            await PerformTestsExpectSuccess(virtualMachine.Sandbox.Id);         
        }

        [Theory]       
        [InlineData(false)]
        [InlineData(true)]
        public async Task IncreasePhase_ToOwnedStudy_AsSponsor_ShouldSucceed(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            var virtualMachine = await WithVirtualMachine(true, restrictedStudy);

            await PerformTestsExpectSuccess(virtualMachine.Sandbox.Id);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task IncreasePhase_ToNonOwnedStudy_AsSponsor_ShouldFail(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            var virtualMachine = await WithVirtualMachine(false, restrictedStudy);        

            await PerformTestsExpectFailure(virtualMachine.Sandbox.Id);
        }

        [Theory]
        [InlineData(false, StudyRoles.SponsorRep)]
        [InlineData(false, StudyRoles.VendorAdmin)]
        [InlineData(true, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.VendorAdmin)]      
        public async Task IncreasePhase_HavingCorrectStudyRoles_ShouldSucceed(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            var virtualMachine = await WithVirtualMachine(false, restrictedStudy, studyRole);
            await PerformTestsExpectSuccess(virtualMachine.Sandbox.Id);
        }

        [Theory]
        [InlineData(false, StudyRoles.VendorContributor)]
        [InlineData(false, StudyRoles.StudyViewer)] 
        [InlineData(true, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer)]
        public async Task IncreasePhase_HavingWrongStudyRoles_ShouldFail(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            var virtualMachine = await WithVirtualMachine(false, restrictedStudy, studyRole);
            await PerformTestsExpectFailure(virtualMachine.Sandbox.Id);
        }

        protected async Task<CloudResource> WithVirtualMachine(bool createdByCurrentUser, bool restricted = false, string studyRole = null)
        {
            return await base.WithVirtualMachine(createdByCurrentUser, restricted, new List<string> { studyRole }, addDatasetsToStudy: true, addDatasetsToSandbox: true);
        }

        async Task PerformTestsExpectSuccess(int sandboxId)
        {
            var sandboxDetailsConversation = await GenericReader.ReadAndAssertExpectSuccess<SandboxDetails>(_restHelper, GenericReader.SandboxDetailsUrl(sandboxId));
            SandboxDetailsAsserts.ReadyForPhaseShiftExpectSuccess(sandboxDetailsConversation.Response);

            var phaseShiftConversation = await GenericPoster.PostAndExpectSuccess<SandboxDetails>(_restHelper, GenericPoster.SandboxNextPhase(sandboxId));
            SandboxDetailsAsserts.AfterPhaseShiftExpectSuccess(phaseShiftConversation.Response);
        }      

        async Task PerformTestsExpectFailure(int sandboxId)
        {
            var sandboxCreateConversation = await GenericPoster.PostAndExpectFailure(_restHelper, GenericPoster.SandboxNextPhase(sandboxId));
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(sandboxCreateConversation.Response);
        }            
    }
}
