using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class ThreadSafeUserCreatorUtil
    {
        static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public static async Task<User> EnsureDbUserExistsAsync(SepesDbContext dbContext,
            ICurrentUserService currentUserService, IAzureUserService azureUserService)
        {
            try
            {
                var loggedInUserObjectId = currentUserService.GetUserId();
                var userFromDb = await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.ObjectId == loggedInUserObjectId);

                if(userFromDb == null)
                {
                    await _semaphore.WaitAsync();

                    userFromDb = await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.ObjectId == loggedInUserObjectId);

                    if(userFromDb == null)
                    {
                        var userFromAzure = await azureUserService.GetUserAsync(loggedInUserObjectId);

                        if (userFromAzure == null)
                        {
                            throw new Exception($"Unable to get info on logged in user from Azure. User id: {loggedInUserObjectId}");
                        }

                        userFromDb = UserUtil.CreateDbUserFromAzureUser(loggedInUserObjectId, userFromAzure);

                        dbContext.Users.Add(userFromDb);
                        await dbContext.SaveChangesAsync();
                    }                  

                    if(userFromDb.StudyParticipants == null)
                    {
                        userFromDb.StudyParticipants = new List<StudyParticipant>();
                    }
                }

                return userFromDb;
            }            
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
