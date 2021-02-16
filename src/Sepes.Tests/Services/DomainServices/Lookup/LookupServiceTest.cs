using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using Sepes.Infrastructure.Dto;
using Xunit.Extensions;
using Sepes.Infrastructure.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [MemberData(nameof(GetData), parameters: 2)]
        [Theory]
        public async void GetStudyRolesNamesUserCanGive(string studyRole, List<LookupDto> expectedValue)
        {
            await RefreshAndSeedTestDatabase(studyRole);
            var lookupService = LookupServiceMockFactory.GetLookupService(_serviceProvider);
            var roles =  await lookupService.StudyRolesUserCanGive(1);
            roles = roles.ToList();

            //Assert.Equal(roles, expectedValue);
            /*
            var result = true;
            foreach(var role in roles)
            {
                if (!expectedValue.Contains(role))
                {
                    result = true;
                }
            }
            */

            CollectionAssert.AreEquivalent(roles.ToList(), expectedValue);
        }

        public static List<object[]> GetData(int numTests)
        {
            var sponsorRepList = new List<LookupDto>();
            sponsorRepList.Add(new LookupDto { Key = StudyRoles.StudyViewer, DisplayValue = StudyRoles.StudyViewer });
            sponsorRepList.Add(new LookupDto { Key = StudyRoles.SponsorRep, DisplayValue = StudyRoles.SponsorRep });
            sponsorRepList.Add(new LookupDto { Key = StudyRoles.VendorAdmin, DisplayValue = StudyRoles.VendorAdmin });
            sponsorRepList.Add(new LookupDto { Key = StudyRoles.VendorContributor, DisplayValue = StudyRoles.VendorContributor });

            var vendorAdmin = new List<LookupDto>();
            vendorAdmin.Add(new LookupDto { Key = StudyRoles.VendorAdmin, DisplayValue = StudyRoles.VendorAdmin });
            vendorAdmin.Add(new LookupDto { Key = StudyRoles.VendorContributor, DisplayValue = StudyRoles.VendorContributor });
            var allData = new List<object[]>
            {
                new object[] { StudyRoles.StudyViewer, new List<LookupDto> { }},
                new object[] { StudyRoles.SponsorRep, sponsorRepList}
                
            };
            //new object[] { StudyRoles.VendorAdmin, vendorAdmin},
            return allData;
            //return allData.Take(numTests);
        }

    }
}
    

