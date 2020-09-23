using Microsoft.Graph;
using Sepes.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IAzureADUsersService
    {
        Task<List<AzureADUserDto>> SearchUsersAsync(string search, int limit);

        Task<User> GetUser(string id);
    }
}
