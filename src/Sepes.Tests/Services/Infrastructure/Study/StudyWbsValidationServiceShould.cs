using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Tests.Common.ModelFactory;
using Sepes.Tests.Common.ServiceMockFactories.Infrastructure;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.Infrastructure
{
    public class StudyWbsValidationServiceShould : ServiceTestBase
    {
        //TODO: Add study tests
        [Fact]
        public async Task AllowStudyCreation_OnValidWbs()
        {
            var study = CreateStudyValidWbs();
            var studyWbsValidationService = StudyWbsValidationMockServiceFactory.GetService(_serviceProvider, true, false);

            await studyWbsValidationService.ValidateForStudyCreateOrUpdate(study);

            Assert.True(study.WbsCodeValid);
        }

        [Fact]
        public async Task AllowStudyCreation_OnInValidWbs_AndSetRelevantProperties()
        {
            var study = CreateStudyInvalidWbs();
            var studyWbsValidationService = StudyWbsValidationMockServiceFactory.GetService(_serviceProvider, false, false);

            await studyWbsValidationService.ValidateForStudyCreateOrUpdate(study);

            Assert.False(study.WbsCodeValid);
        }

        [Fact]
        public async Task AllowStudyCreation_OnFailedWbsValidation_AndSetRelevantProperties()
        {
            var study = CreateStudyInvalidWbs();
            var studyWbsValidationService = StudyWbsValidationMockServiceFactory.GetService(_serviceProvider, true, true);

            await studyWbsValidationService.ValidateForStudyCreateOrUpdate(study);

            Assert.False(study.WbsCodeValid);
        }

        [Fact]
        public async Task AllowSandboxAndDatasetCreation_OnValidWbs()
        {
            var study = CreateStudyValidWbs();
            var studyWbsValidationService = StudyWbsValidationMockServiceFactory.GetService(_serviceProvider, true, false);

            await studyWbsValidationService.ValidateForSandboxCreationOrThrow(study);
            await studyWbsValidationService.ValidateForDatasetCreationOrThrow(study);

            Assert.True(study.WbsCodeValid);
        }       

        [Fact]
        public async Task AllowSandboxCreation_And_UpdateWbsValidationFields_IfReValidationSucceeds()
        {
            var study = CreateStudyInvalidWbs();
            var studyWbsValidationService = StudyWbsValidationMockServiceFactory.GetService(_serviceProvider, true, false);

            await studyWbsValidationService.ValidateForSandboxCreationOrThrow(study);
            await studyWbsValidationService.ValidateForDatasetCreationOrThrow(study);

            Assert.True(study.WbsCodeValid);
           Assert.NotNull((study.WbsCodeValidatedAt));
           Assert.True((DateTime.UtcNow - study.WbsCodeValidatedAt.Value).TotalSeconds < 10);
        }
        
        [Fact]
        public async Task Throw_On_SandboxAndDatasetCreation_IfInvalidAndReValidationReturnsFalse()
        {
            var study = CreateStudyInvalidWbs();
            var studyWbsValidationService = StudyWbsValidationMockServiceFactory.GetService(_serviceProvider, false, false);

            await Assert.ThrowsAsync<InvalidWbsException>(() => studyWbsValidationService.ValidateForSandboxCreationOrThrow(study));
            await Assert.ThrowsAsync<InvalidWbsException>(() => studyWbsValidationService.ValidateForDatasetCreationOrThrow(study));
        }
        
        [Fact]
        public async Task ThrowIfInvalidAndReValidationThrows()
        {
            var study = CreateStudyInvalidWbs();
            var studyWbsValidationService = StudyWbsValidationMockServiceFactory.GetService(_serviceProvider, false, true);
           
            await Assert.ThrowsAsync<InvalidWbsException>(() => studyWbsValidationService.ValidateForSandboxCreationOrThrow(study));
            await Assert.ThrowsAsync<InvalidWbsException>(() => studyWbsValidationService.ValidateForDatasetCreationOrThrow(study));
        }

        Study CreateStudyValidWbs()
        {
            return StudyModelFactory.CreateBasic();
        }

        Study CreateStudyInvalidWbs()
        {
            return StudyModelFactory.CreateBasic(wbsValid: false);
        }       
    }
}
