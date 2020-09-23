using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Util;
using System.Linq;
using Xunit;

namespace Sepes.Tests.Util
{
    public class StudyAccessUtilTest_MultipleStudy : StudyAccessUtilTest_Base
    {
        public StudyAccessUtilTest_MultipleStudy()
            : base()
        {

        }


        [Fact]
        public async void GettingListOfStudies_AsUserWithoutRoles_ShouldOnlyReturnUnrestrictedStudies()
        {
            var db = GetContextWithAdvancedTestData();

            var studiesForUser1 = await StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(db, 1).ToListAsync();
            Assert.Equal(6, studiesForUser1.Count);

            Assert.False(studiesForUser1.Where(s => s.Restricted).Any());
        }

        [Fact]
        public async void GettingListOfStudies_AsUserWithSomeRoles_ShouldReturnUnrestrictedAndThoseIHaveAccessToStudies()
        {
            var db = GetContextWithAdvancedTestData();

            var studiesForUser2 = await StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(db, 2).ToListAsync();
            Assert.Equal(6, studiesForUser2.Count);

            var studiesForUser3 = await StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(db, 3).ToListAsync();
            Assert.Equal(11, studiesForUser3.Count);

            var studiesForUser4 = await StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(db, 4).ToListAsync();
            Assert.Equal(7, studiesForUser4.Count);

            var studiesForUser5 = await StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(db, 5).ToListAsync();
            Assert.Equal(7, studiesForUser5.Count);

            var studiesForUser6 = await StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(db, 6).ToListAsync();
            Assert.Equal(7, studiesForUser6.Count);

            var studiesForUser7 = await StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(db, 7).ToListAsync();
            Assert.Equal(7, studiesForUser7.Count);

            var studiesForUser8 = await StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(db, 8).ToListAsync();
            Assert.Equal(8, studiesForUser8.Count);
        }

    }
}
