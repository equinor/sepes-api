using Sepes.Common.Constants;
using Sepes.Common.Dto.Dataset;
using Sepes.Common.Dto.Storage;
using Sepes.Common.Dto.Study;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            await WithBasicSeeds();
            var virtualMachine = await WithVirtualMachine(createdByCurrentUser, restrictedStudy, addDatasetsToStudy: true, addDatasetsToSandbox: true);
            await ReadAllAndAssertExpectSuccess(virtualMachine);
            Trace.WriteLine("END Read_AnyStudyRelatedEntity_AsAdmin_ShouldSucceed");
        }

        [Theory]
        [InlineData( false)]
        [InlineData( true)]    
        public async Task Read_OwnedStudyRelatedEntity_AsSponsor_ShouldSucceed(bool restrictedStudy)
        {
            SetScenario(isSponsor: true);
            await WithBasicSeeds();
            var virtualMachine = await WithVirtualMachine(true, restrictedStudy, addDatasetsToStudy: true, addDatasetsToSandbox: true);
            await ReadAllAndAssertExpectSuccess(virtualMachine);
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
            await WithBasicSeeds();
            var virtualMachine = await WithVirtualMachine(false, restrictedStudy, new List<string> { studyRole }, addDatasetsToStudy: true, addDatasetsToSandbox: true);
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
            await WithBasicSeeds();

            var virtualMachine = await WithVirtualMachine(false, true, addDatasetsToStudy: true, addDatasetsToSandbox: true);

            await ReadAllAndAssertExpectForbidden(virtualMachine);
        }       


        async Task ReadAllAndAssertExpectSuccess(CloudResource vmResource)
        {
            await GenericReader.ReadAndAssertExpectSuccess<StudyDetailsDto>(_restHelper, GenericReader.StudyDetailsUrl(vmResource.Sandbox.StudyId));

            //Dataset
            var datasetId = vmResource.Sandbox.Study.StudyDatasets.FirstOrDefault().DatasetId;           
            await GenericReader.ReadAndAssertExpectSuccess<DatasetDto>(_restHelper, GenericReader.StudyDatasetSpecificUrl(vmResource.Sandbox.StudyId, datasetId));
            await GenericReader.ReadAndAssertExpectSuccess<List<DatasetResourceLightDto>>(_restHelper, GenericReader.StudyDatasetResourcesUrl(vmResource.Sandbox.StudyId, datasetId));          
            await GenericReader.ReadAndAssertExpectSuccess<List<BlobStorageItemDto>>(_restHelper, GenericReader.DatasetFileListUrl(datasetId));

            await GenericReader.ReadAndAssertExpectSuccess<SandboxDetails>(_restHelper, GenericReader.SandboxDetailsUrl(vmResource.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectSuccess<List<SandboxResourceLight>>(_restHelper, GenericReader.SandboxResourcesUrl(vmResource.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectSuccess<string>(_restHelper, GenericReader.SandboxCostAnalysisUrl(vmResource.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectSuccess<AvailableDatasets>(_restHelper, GenericReader.SandboxAvailableDatasetsUrl(vmResource.Sandbox.Id));            

            await GenericReader.ReadAndAssertExpectSuccess<List<VmDto>>(_restHelper, GenericReader.SandboxVirtualMachinesUrl(vmResource.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectSuccess<VmExtendedDto>(_restHelper, GenericReader.VirtualMachineExtendedInfoUrl(vmResource.Id));
            await GenericReader.ReadAndAssertExpectSuccess<VmExternalLink>(_restHelper, GenericReader.VirtualMachineExternalLinkUrl(vmResource.Id));
            await GenericReader.ReadAndAssertExpectSuccess<List<VmRuleDto>>(_restHelper, GenericReader.VirtualMachineRulesUrl(vmResource.Id));
        }

        async Task ReadAllAndAssertExpectForbidden(CloudResource vmResource) {

            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.StudyDetailsUrl(vmResource.Sandbox.StudyId));           

            //Study dataset
            var datasetId = vmResource.Sandbox.Study.StudyDatasets.FirstOrDefault().DatasetId;
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.StudyDatasetSpecificUrl(vmResource.Sandbox.StudyId, datasetId));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.StudyDatasetResourcesUrl(vmResource.Sandbox.StudyId, datasetId));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.DatasetFileListUrl(datasetId));

            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxDetailsUrl(vmResource.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxResourcesUrl(vmResource.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxCostAnalysisUrl(vmResource.Sandbox.Id));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxAvailableDatasetsUrl(vmResource.Sandbox.Id));

            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.SandboxVirtualMachinesUrl(vmResource.Sandbox.Id));         
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.VirtualMachineExtendedInfoUrl(vmResource.Id));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.VirtualMachineExternalLinkUrl(vmResource.Id));
            await GenericReader.ReadAndAssertExpectForbidden(_restHelper, GenericReader.VirtualMachineRulesUrl(vmResource.Id));
        }       
    }
}
