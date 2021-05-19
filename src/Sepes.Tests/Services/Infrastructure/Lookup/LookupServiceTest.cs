using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Tests.Setup;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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

            Xunit.Assert.Equal(roles.Count(), expectedValue);
        }

        [MemberData(nameof(GetData))]
        [Theory]
        public async void GetStudyRolesUserCanGive(string studyRole, List<LookupDto> expectedRoles)
        {
            await RefreshAndSeedTestDatabase(studyRole);
            var lookupService = LookupServiceMockFactory.GetLookupService(_serviceProvider);
            var roles = await lookupService.StudyRolesUserCanGive(1);
            var rolesList = roles.ToList();

            Xunit.Assert.Equal(expectedRoles.Count, rolesList.Count);

            CollectionAssert.AreEquivalent(expectedRoles, rolesList);
        }

        public static List<object[]> GetData()
        {
            var assignableByStudyOwnerAndSponsorRep = new List<LookupDto>();
            assignableByStudyOwnerAndSponsorRep.Add(new LookupDto { Key = StudyRoles.StudyViewer, DisplayValue = StudyRoles.StudyViewer });
            assignableByStudyOwnerAndSponsorRep.Add(new LookupDto { Key = StudyRoles.SponsorRep, DisplayValue = StudyRoles.SponsorRep });
            assignableByStudyOwnerAndSponsorRep.Add(new LookupDto { Key = StudyRoles.VendorAdmin, DisplayValue = StudyRoles.VendorAdmin });
            assignableByStudyOwnerAndSponsorRep.Add(new LookupDto { Key = StudyRoles.VendorContributor, DisplayValue = StudyRoles.VendorContributor });

            var assignableByVendorAdmin = new List<LookupDto>();
            assignableByVendorAdmin.Add(new LookupDto { Key = StudyRoles.VendorAdmin, DisplayValue = StudyRoles.VendorAdmin });
            assignableByVendorAdmin.Add(new LookupDto { Key = StudyRoles.VendorContributor, DisplayValue = StudyRoles.VendorContributor });

            var assignableByVendorContributor = new List<LookupDto>();
            assignableByVendorContributor.Add(new LookupDto { Key = StudyRoles.VendorContributor, DisplayValue = StudyRoles.VendorContributor });

            var allData = new List<object[]>
            {
                new object[] { StudyRoles.StudyViewer, new List<LookupDto> { }},
                new object[] { StudyRoles.StudyOwner, assignableByStudyOwnerAndSponsorRep},
                new object[] { StudyRoles.SponsorRep, assignableByStudyOwnerAndSponsorRep},
                new object[] { StudyRoles.VendorAdmin, assignableByVendorAdmin},
                new object[] { StudyRoles.VendorContributor, assignableByVendorContributor }
            };

            return allData;
        }
    }
}


