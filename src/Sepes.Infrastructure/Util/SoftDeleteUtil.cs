using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class SoftDeleteUtil
    {
        public static void MarkAsDeleted(ISupportSoftDelete itemToDelete, string deletedBy)
        {
            if (itemToDelete == null)
            {
                throw new ArgumentException("Item to delete was null");
            }
            if(!IsMarkedAsDeleted(itemToDelete))
            {
                itemToDelete.Deleted = true;
                itemToDelete.DeletedBy = deletedBy;
                itemToDelete.DeletedAt = DateTime.UtcNow;
            }           
        }

        public static void MarkAsDeleted(ISupportSoftDelete itemToDelete, User deletedByUser)
        {
            var deletedBy = deletedByUser.UserName;
            MarkAsDeleted(itemToDelete, deletedBy);
        }

        public static void MarkAsDeleted(ISupportSoftDelete itemToDelete, UserDto deletedByUser)
        {
            var deletedBy = deletedByUser.UserName;
            MarkAsDeleted(itemToDelete, deletedBy);
        }

        public static async Task MarkAsDeleted(ISupportSoftDelete itemToDelete, IUserService userService)
        {
            var deletedByUser = await userService.GetCurrentUserAsync();
            MarkAsDeleted(itemToDelete, deletedByUser);
        }

        public static bool IsMarkedAsDeleted(ISupportSoftDelete entry)
        {
            return entry.Deleted;
        }
    }
}
