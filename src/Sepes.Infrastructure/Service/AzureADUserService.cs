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
        readonly IStudyParticipantService _studyParticipantService;

        public AzureADUsersService(IGraphServiceProvider graphServiceProvider, IMapper mapper, IStudyParticipantService studyParticipantService)
        {
            _mapper = mapper;
            _graphServiceProvider = graphServiceProvider;
            _studyParticipantService = studyParticipantService;
        }
        public async Task<List<AzureADUserDto>> SearchUsersAsync(string search, int limit)
        {
            // Initialize the GraphServiceClient.            
            GraphServiceClient graphClient = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All" });

            var result = await graphClient.Users.Request().Top(limit).Filter($"startswith(displayName,'{search}') or startswith(givenName,'{search}') or startswith(surname,'{search}') or startswith(mail,'{search}') or startswith(userPrincipalName,'{search}')").GetAsync();
            return _mapper.Map<List<AzureADUserDto>>(result);
        }
        public async Task<User> GetUser(string id)
        {
            // Initialize the GraphServiceClient. 
            GraphServiceClient graphClient = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All" });

            var result = await graphClient.Users.Request().Filter($"Id eq '{id}'").GetAsync();

            return result.FirstOrDefault();
        }


    }
}
