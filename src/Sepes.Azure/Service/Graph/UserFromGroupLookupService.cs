using Microsoft.Graph;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class UserFromGroupLookupService : IUserFromGroupLookupService
    {
        string[] SCOPES = new[] { "GroupMember.Read.All" };
        readonly IGraphServiceProvider _graphServiceProvider;

        public UserFromGroupLookupService(IGraphServiceProvider graphServiceProvider)
        {
            _graphServiceProvider = graphServiceProvider;
        }

        public async Task<Dictionary<string, AzureUserDto>> SearchInGroupAsync(string groupId, string search, int limit, CancellationToken cancellationToken = default)
        {
            var userList = new Dictionary<string, AzureUserDto>();

            if (string.IsNullOrWhiteSpace(search))
            {
                return userList;
            }

            // Initialize the GraphServiceClient.            
            var graphClient = _graphServiceProvider.GetGraphServiceClient(SCOPES);

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("$count", "true"),
                new QueryOption("$search", $"\"displayName:{search}\" OR \"surname:{search}\" OR \"mail:{search}\" OR \"userPrincipalName:{search}\"")
            };

            var graphRequest = graphClient.Groups[groupId].Members
                .Request(queryOptions)
                .Header("ConsistencyLevel", "eventual")
                .Select("displayName,id,mail,userPrincipalName")
                .Top(limit);

            while (true)
            {
                if (graphRequest == null || userList.Count > limit)
                {
                    break;
                }

                var response = await graphRequest.GetAsync(cancellationToken: cancellationToken);

                foreach (var curItem in response.CurrentPage)
                {
                    var itemAsUser = curItem as User;

                    if (itemAsUser != null)
                    {
                        if (!userList.ContainsKey(itemAsUser.Id))
                        {
                            userList.Add(itemAsUser.Id, new AzureUserDto() { Id = itemAsUser.Id, Mail = itemAsUser.Mail, DisplayName = itemAsUser.DisplayName, UserPrincipalName = itemAsUser.UserPrincipalName });
                        }
                    }
                }

                graphRequest = response.NextPageRequest;
            }

            return userList;
        }
    }
}
