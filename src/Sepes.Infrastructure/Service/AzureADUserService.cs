using AutoMapper;
using Microsoft.Graph;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<List<Microsoft.Graph.User>> SearchUsersAsync(string search, int limit)
        {
            List<Microsoft.Graph.User> listUsers = new List<User>();

            if (string.IsNullOrWhiteSpace(search))
            {
                return listUsers;
            }

            // Initialize the GraphServiceClient.            
            GraphServiceClient graphClient = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All" });

            var graphRequest = graphClient.Users.Request().Top(limit).Filter($"startswith(displayName,'{search}') or startswith(givenName,'{search}') or startswith(surname,'{search}') or startswith(mail,'{search}') or startswith(userPrincipalName,'{search}')");

            
            while (true)
            {
                if(graphRequest == null || listUsers.Count > limit)
                {
                    break;
                }
                var response = await graphRequest.GetAsync();

                 listUsers.AddRange(response.CurrentPage);
                    
                
                graphRequest = response.NextPageRequest;
            } 

            return listUsers;

        }
        public async Task<User> GetUser(string id)
        {
            // Initialize the GraphServiceClient. 
            GraphServiceClient graphClient = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All" });

            var result = await graphClient.Users.Request().Filter($"Id eq '{id}'").GetAsync();

            return result.FirstOrDefault();
        }

        public Task<IEnumerable<User>> SearchUsersAsync2(string search, int limit)
        {
            throw new System.NotImplementedException();
        }
    }
}
