using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class StudySeed
    {
        public static async Task<Study> CreatedByCurrentUser(string name = StudyConstants.CREATED_BY_ME_NAME, string vendor = StudyConstants.CREATED_BY_ME_VENDOR, string wbs = StudyConstants.CREATED_BY_ME_WBS, bool restricted = false, int userId = TestUserConstants.COMMON_CUR_USER_DB_ID)
        {
            var study = StudyBasic(name, vendor, wbs, restricted);

            AddParticipant(study, userId, StudyRoles.StudyOwner);

           return await SliceFixture.InsertAsync(study);
        }

        public static async Task<Study> CreatedByOtherUser(
            string name = StudyConstants.CREATED_BY_OTHER_NAME,
            string vendor = StudyConstants.CREATED_BY_OTHER_VENDOR,
            string wbs = StudyConstants.CREATED_BY_OTHER_WBS,
            bool restricted = false,
            int ownerUserId = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_DB_ID,
            int userId = TestUserConstants.COMMON_CUR_USER_DB_ID,
            string currentUserRole = null)
        {
            var study = StudyBasic(name, vendor, wbs, restricted);

            AddParticipant(study, ownerUserId, StudyRoles.StudyOwner);

            if (!String.IsNullOrWhiteSpace(currentUserRole))
            {
            AddParticipant(study, userId, currentUserRole);
            }         

            return await SliceFixture.InsertAsync(study);             
        }

        static Study StudyBasic(string name, string vendor, string wbs, bool restricted)
        {
            return new Study()
            {             
                Name = name,
                Vendor = vendor,
                WbsCode = wbs,
                Restricted = restricted,
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow,
            };
        }

        public static void AddParticipant(Study study, int userId, string role)
        {
            if (study.StudyParticipants == null)
            {
                study.StudyParticipants = new List<StudyParticipant>();
            }

            var newParticipant = new StudyParticipant()
            {
                UserId = userId,
                RoleName = role,
                CreatedBy = "seed",
                Created = DateTime.UtcNow               
            };

            study.StudyParticipants.Add(newParticipant);
        }
    }
}
