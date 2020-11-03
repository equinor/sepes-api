using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System;

namespace Sepes.Tests.Setup
{
    public static class UserPopulator
    {
        public static User Add(SepesDbContext db, string objectId, string userName, string email, string fullName)
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
