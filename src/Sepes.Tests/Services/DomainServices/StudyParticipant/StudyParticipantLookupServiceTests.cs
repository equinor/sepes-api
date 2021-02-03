using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace Sepes.Tests.Services.DomainServices.StudyParticipant
{
    public class StudyParticipantLookupServiceTests : StudyParticipantLookupBase
    {
        public StudyParticipantLookupServiceTests()
           : base()
        {

        }

        [Fact]
        public async void GetParticipantsWithNameFrom_GetLookupAsync()
        {
            await RefreshAndSeedTestDatabase();
            var studyParticipantLookupService = StudyParticipantMockFactory.GetStudyParticipantLookupService(_serviceProvider);
            var usersWithName = await studyParticipantLookupService.GetLookupAsync("John");
            
            Assert.Equal(2, usersWithName.Count());
        }

        [Fact]
        public async void GetParticipantsWithEmailFrom_GetLookupAsync()
        {
            await RefreshAndSeedTestDatabase();
            var studyParticipantLookupService = StudyParticipantMockFactory.GetStudyParticipantLookupService(_serviceProvider);
            var usersWithName = await studyParticipantLookupService.GetLookupAsync("John@hotmail.com");
            Assert.Single(usersWithName);
        }

        [Fact]
        public async void GetInvalidNameParticipantsFrom_GetLookupAsync()
        {
            await RefreshAndSeedTestDatabase();
            var studyParticipantLookupService = StudyParticipantMockFactory.GetStudyParticipantLookupService(_serviceProvider);
            var usersWithName = await studyParticipantLookupService.GetLookupAsync("No Person Has this name");
            Assert.Empty(usersWithName);
        }
    }
}
