using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Tests.Common.Constants;
using System;

namespace Sepes.Tests.Setup
{
    public static class UserPopulator
    {
        public static User Add(SepesDbContext db, string objectId = TestUserConstants.COMMON_CUR_USER_OBJECTID, string userName = TestUserConstants.COMMON_CUR_USER_UPN, string email = TestUserConstants.COMMON_CUR_USER_EMAIL, string fullName = TestUserConstants.COMMON_CUR_USER_FULL_NAME)
        {
            var newUser = new User()
            {
                ObjectId = objectId,
                UserName = userName, 
                EmailAddress = email,
                FullName = fullName,
                Created = DateTime.UtcNow,
                CreatedBy = "unittest"
            };          

            db.Users.Add(newUser);

            db.SaveChanges();

            return newUser;
        }
    }
}
