using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Tests.Constants;
using Sepes.Tests.Setup;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sepes.Tests.Services.DomainServices.Queries
{
    public class StudyQueriesTest_Base
    {
        protected const int COMMON_STUDY_ID = 2;

        protected ServiceCollection Services;
        protected ServiceProvider ServiceProvider;

        public StudyQueriesTest_Base()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            ServiceProvider = Services.BuildServiceProvider();
        }

        protected SepesDbContext GetEmptyContext()
        {
            var db = ServiceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        protected SepesDbContext GetContextWithSimpleTestData(int userId, int studyId, bool restricted, params string[] studySpecificRoles)
        {
            var db = GetEmptyContext();

            DecorateWithUser(db, userId);
            DecorateWithStudy(db, studyId, restricted, userId, studySpecificRoles);

            db.SaveChanges();

            return db;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        protected SepesDbContext GetContextWithAdvancedTestData()
        {
            var db = GetEmptyContext();

            var user1 = DecorateWithUser(db, 1);
            var user2 = DecorateWithUser(db, 2);
            var user3 = DecorateWithUser(db, 3);

            //Some UN-restricted studies
            var study21 = DecorateWithStudy(db, 21, false, user2.Id);
            var study22 = DecorateWithStudy(db, 22, false, user2.Id, StudyRoles.StudyOwner);
            var study23 = DecorateWithStudy(db, 23, false, user2.Id, StudyRoles.StudyViewer);
            var study24 = DecorateWithStudy(db, 24, false, user2.Id, StudyRoles.VendorAdmin);
            var study25 = DecorateWithStudy(db, 25, false, user2.Id, StudyRoles.VendorContributor);
            var study26 = DecorateWithStudy(db, 26, false, user2.Id, StudyRoles.SponsorRep);

            //Some restricted studies
            var study31 = DecorateWithStudy(db, 31, true, user3.Id);
            var study32 = DecorateWithStudy(db, 32, true, user3.Id, StudyRoles.StudyOwner);
            var study33 = DecorateWithStudy(db, 33, true, user3.Id, StudyRoles.StudyViewer);
            var study34 = DecorateWithStudy(db, 34, true, user3.Id, StudyRoles.VendorAdmin);
            var study35 = DecorateWithStudy(db, 35, true, user3.Id, StudyRoles.VendorContributor);
            var study36 = DecorateWithStudy(db, 36, true, user3.Id, StudyRoles.SponsorRep);

            var user4 = DecorateWithUser(db, 4);
            var study41 = DecorateWithStudy(db, 41, true, user4.Id, StudyRoles.StudyOwner);

            var user5 = DecorateWithUser(db, 5);
            var study51 = DecorateWithStudy(db, 51, true, user5.Id, StudyRoles.StudyViewer);

            var user6 = DecorateWithUser(db, 6);
            var study61 = DecorateWithStudy(db, 61, true, user6.Id, StudyRoles.VendorAdmin);

            var user7 = DecorateWithUser(db, 7);
            var study71 = DecorateWithStudy(db, 71, true, user7.Id, StudyRoles.VendorContributor);

            var user8 = DecorateWithUser(db, 8);
            var study81 = DecorateWithStudy(db, 81, true, user8.Id, StudyRoles.SponsorRep);

            db.SaveChanges();

            return db;
        }

        protected User DecorateWithUser(SepesDbContext db, int userId)
        {
            var user = new User() { Id = userId, UserName = $"Test user with Id {userId}" };
            db.Users.Add(user);
            return user;
        }

        protected Study DecorateWithStudy(SepesDbContext db, int studyId, bool restricted, int userId, params string[] studySpecificRoles)
        {
            var study = new Study()
            {
                Id = studyId,
                Name = $"TestStudy with Id {studyId}, Restricted: {restricted}",
                Restricted = restricted,
                StudyParticipants = new List<Sepes.Infrastructure.Model.StudyParticipant>()
            };

            db.Studies.Add(study);

            DecorateStudyWithParticipant(study, userId, studySpecificRoles);

            return study;
        }

        protected void DecorateStudyWithParticipant(Study study, int userId, params string[] studySpecificRoles)
        {
            foreach (var curRole in studySpecificRoles)
            {
                study.StudyParticipants.Add(new Sepes.Infrastructure.Model.StudyParticipant() { StudyId = study.Id, UserId = userId, RoleName = curRole });
            }
        }

        protected void PerformUsualStudyTests(Study study)
        {
            Assert.NotNull(study);
            Assert.Equal(COMMON_STUDY_ID, study.Id);

            Assert.NotNull(study.StudyParticipants);
        }

        protected void UserMustBeAmongStudyParticipants(Study study)
        {
            Assert.NotEmpty(study.StudyParticipants);

            var studyParticipant = study.StudyParticipants.FirstOrDefault();
            Assert.NotNull(studyParticipant);
            Assert.NotNull(studyParticipant.User);
            Assert.Equal(UserConstants.COMMON_CUR_USER_DB_ID, studyParticipant.User.Id);
        }
    }
}
