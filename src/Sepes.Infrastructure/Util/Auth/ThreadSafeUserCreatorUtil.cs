using Microsoft.EntityFrameworkCore;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Interface;
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
                await _semaphore.WaitAsync();

                var loggedInUserObjectId = currentUserService.GetUserId();

                var userFromDb = await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.ObjectId == loggedInUserObjectId);

                if(userFromDb == null)
                {
                    AzureUserDto userFromAzure = null;
                    if (loggedInUserObjectId.Equals(""))
                    {
                        userFromAzure = new AzureUserDto
                        {
                            DisplayName = "Mock User",
                            UserPrincipalName = "mock@user.com",
                            Mail = "mock@user.com"
                        };
                    }
                    else
                    {
                        userFromAzure = await azureUserService.GetUserAsync(loggedInUserObjectId);

                        if (userFromAzure == null)
                        {
                            throw new Exception($"Unable to get info on logged in user from Azure. User id: {loggedInUserObjectId}");
                        }
                    }
                    
                    userFromDb = UserUtil.CreateDbUserFromAzureUser(loggedInUserObjectId, userFromAzure);

                    dbContext.Users.Add(userFromDb);
                    await dbContext.SaveChangesAsync();

                    if (userFromDb.StudyParticipants == null)
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
