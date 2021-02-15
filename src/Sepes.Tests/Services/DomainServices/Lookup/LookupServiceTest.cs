using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace Sepes.Tests.Services.DomainServices.Lookup
{
    public class LookupServiceTest : LookupServiceBase
    {
        public LookupServiceTest()
           : base()
        {
        }

        [InlineData(4)]
        [Theory]
        public void GetStudyRolesNames(int expectedValue)
        {
            var lookupService = LookupServiceMockFactory.GetLookupService(_serviceProvider);
            var roles = lookupService.StudyRoles();

            Assert.Equal(roles.Count(), expectedValue);
        }

        [InlineData(4, Infrastructure.Constants.StudyRoles.SponsorRep)]
        [InlineData(3, Infrastructure.Constants.StudyRoles.VendorAdmin)]
        [InlineData(2, Infrastructure.Constants.StudyRoles.VendorContributor)]
        [InlineData(1, Infrastructure.Constants.StudyRoles.StudyViewer)]
        [Theory]
        public async void GetStudyRolesNamesUserCanGive(int expectedValue, string studyRole)
        {
            await RefreshAndSeedTestDatabase(studyRole);
            var lookupService = LookupServiceMockFactory.GetLookupService(_serviceProvider);
            var roles =  await lookupService.StudyRolesUserCanGive(1);

            Assert.Equal(roles.Count(), expectedValue);
        }

    }
}
    

