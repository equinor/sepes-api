using AutoMapper;
using Microsoft.Graph;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureADUsersService : IAzureADUsersService
    {
        private readonly IGraphServiceProvider _graphServiceProvider;
        private readonly IMapper _mapper;

        public AzureADUsersService(IGraphServiceProvider graphServiceProvider, IMapper mapper)
        {
            _mapper = mapper;
            _graphServiceProvider = graphServiceProvider;
        }
        public async Task<List<AzureADUserDto>> SearchUsers(string search, int limit)
        {
            // Initialize the GraphServiceClient.            
            GraphServiceClient graphClient = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All" });

            var result = await graphClient.Users.Request().Top(limit).Filter($"startswith(displayName,'{search}') or startswith(givenName,'{search}') or startswith(surname,'{search}') or startswith(mail,'{search}') or startswith(userPrincipalName,'{search}')").GetAsync();

            return _mapper.Map<List<AzureADUserDto>>(result);
        }

    }
}
