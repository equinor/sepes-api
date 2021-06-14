using Microsoft.Graph;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class UserFromGroupLookupService
    {
        readonly string _scope;
        readonly IGraphServiceProvider _graphServiceProvider;
        
        public UserFromGroupLookupService(string scope, IGraphServiceProvider graphServiceProvider)
        {
            _scope = scope;
            _graphServiceProvider = graphServiceProvider; 
        }

       protected async Task<Dictionary<string, AzureUserDto>> SearchInternalAsync(string groupId, string search, int limit, CancellationToken cancellationToken = default)
        {
            var userList = new Dictionary<string, AzureUserDto>();

            if (string.IsNullOrWhiteSpace(search))
            {
                return userList;
            }

            // Initialize the GraphServiceClient.            
            var graphClient = _graphServiceProvider.GetGraphServiceClient(new[] { _scope });

            var graphRequest = graphClient.Groups[groupId].Members
                .Request()
                .Top(limit)
                .Filter($"startswith(displayName,'{search}') or startswith(givenName,'{search}') or startswith(surname,'{search}') or startswith(mail,'{search}') or startswith(userPrincipalName,'{search}')")
                ;
              
                        
            while (true)
            {
                if(graphRequest == null || userList.Count > limit)
                {
                    break;
                }

                var response = await graphRequest.GetAsync(cancellationToken: cancellationToken);              

                foreach(var curItem in response.CurrentPage)
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
