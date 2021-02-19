using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.Constants;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class UserSeed
    {
        public static async Task Seed()
        {
            var testUser = new Infrastructure.Model.User()
            {
                Id = 1,
                EmailAddress = TestUserConstants.COMMON_CUR_USER_EMAIL,
                ObjectId = TestUserConstants.COMMON_CUR_USER_OBJECTID,
                FullName = TestUserConstants.COMMON_CUR_USER_FULL_NAME,
                UserName = TestUserConstants.COMMON_CUR_USER_UPN,
                Created = DateTime.UtcNow,
                CreatedBy = "seed",
                Updated = DateTime.UtcNow,
                UpdatedBy = "seed"

            };

            await SliceFixture.InsertAsync(testUser);

            var someOtherParticipant = new Infrastructure.Model.User()
            {
                Id = 2,
                EmailAddress = TestUserConstants.COMMON_NEW_PARTICIPANT_EMAIL,
                ObjectId = TestUserConstants.COMMON_NEW_PARTICIPANT_OBJECTID,
                FullName = TestUserConstants.COMMON_NEW_PARTICIPANT_FULL_NAME,
                UserName = TestUserConstants.COMMON_NEW_PARTICIPANT_UPN,
                Created = DateTime.UtcNow,
                CreatedBy = "seed",
                Updated = DateTime.UtcNow,
                UpdatedBy = "seed"

            };

            await SliceFixture.InsertAsync(someOtherParticipant);

            var someOtherStudyOwner = new Infrastructure.Model.User()
            {
                Id = 3,
                EmailAddress = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_EMAIL,
                ObjectId = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_OBJECTID,
                FullName = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_FULL_NAME,
                UserName = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_UPN,
                Created = DateTime.UtcNow,
                CreatedBy = "seed",
                Updated = DateTime.UtcNow,
                UpdatedBy = "seed"

            };

            await SliceFixture.InsertAsync(someOtherStudyOwner);


        }
    }
}
