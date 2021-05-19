using Sepes.Common.Dto.Study;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Diagnostics;
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
        

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task Read_AnyStudyRelatedEntity_AsAdmin_ShouldSucceed(bool createdByCurrentUser, bool restrictedStudy)
        {
            Trace.WriteLine("START Read_AnyStudyRelatedEntity_AsAdmin_ShouldSucceed");
            SetScenario(isAdmin: true);
            await WithUserSeeds();
            var virtualMachine = await WithVirtualMachine(createdByCurrentUser, restrictedStudy);
            await ReadAllAndAssertExpectSuccess(virtualMachine);
            Trace.WriteLine("END Read_AnyStudyRelatedEntity_AsAdmin_ShouldSucceed");
        }

        [Theory]
        [InlineData( false)]
        [InlineData( true)]    
        public async Task Read_OwnedStudyRelatedEntity_AsSponsor_ShouldSucceed(bool restrictedStudy)
        {
            SetScenario(isSponsor: true);
            await WithUserSeeds();
            var virtualMachine = await WithVirtualMachine(true, restrictedStudy);
            await ReadAllAndAssertExpectSuccess(virtualMachine);
        }

        [Theory]
        [InlineData(false)]
        //[InlineData(false, StudyRoles.SponsorRep)]
        //[InlineData(false, StudyRoles.VendorAdmin)]
        //[InlineData(false, StudyRoles.VendorContributor)]
        //[InlineData(false, StudyRoles.StudyViewer)]
        //[InlineData(true, StudyRoles.SponsorRep)]
        //[InlineData(true, StudyRoles.VendorAdmin)]
        //[InlineData(true, StudyRoles.VendorContributor)]
        //[InlineData(true, StudyRoles.StudyViewer)]

        public async Task Read_AnyStudyRelatedEntity_WithCorrectStudyRole_ShouldSucceed(bool restrictedStudy, string studyRole = null)
        {
            SetScenario(isEmployee: true);
            await WithUserSeeds();
            var virtualMachine = await WithVirtualMachine(false, restrictedStudy, new List<string> { studyRole });
            await ReadAllAndAssertExpectSuccess(virtualMachine);                  
        }     


        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]       
        public async Task Read_RestrictedStudyRelatedEntity_WithoutRelevantRoles_ShouldFail(bool employee, bool sponsor, bool datasetAdmin)
        {
            SetScenario(isEmployee: employee, isSponsor: sponsor, isDatasetAdmin: datasetAdmin);
            await WithUserSeeds();

            var virtualMachine = await WithVirtualMachine(false, true);

            await ReadAllAndAssertExpectForbidden(virtualMachine);
        }

        async Task ReadAllAndAssertExpectSuccess(CloudResource vm)
        {
            await GenericReader.ReadAndAssertExpectSuccess<StudyDetailsDto>(_restHelper, GenericReader.StudyUrl(vm.Sandbox.StudyId));
            await GenericReader.ReadAndAssertExpectSuccess<SandboxDetails>(_restHelper, GenericReader.SandboxUrl(vm.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectSuccess<List<VmDto>>(_restHelper, GenericReader.SandboxVirtualMachines(vm.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectSuccess<List<SandboxResourceLight>>(_restHelper, GenericReader.SandboxResources(vm.Sandbox.Id));
            //await GenericReader.ReadAndAssertExpectSuccess<VmExtendedDto>(_restHelper, GenericReader.VirtualMachineExtendedInfo(vm.Id)); //Todo: Add this test, but remember to mock out azure vm service
        }

        async Task ReadAllAndAssertExpectForbidden(CloudResource vm) {
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.StudyUrl(vm.Sandbox.StudyId));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxUrl(vm.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxVirtualMachines(vm.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxResources(vm.Sandbox.Id));
            // await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.VirtualMachineExtendedInfo(vm.Id)); //Todo: Add this test, but remember to mock out azure vm service
        }       
    }
}
