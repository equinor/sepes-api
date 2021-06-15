using AutoMapper;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureUserService : IAzureUserService
    {
        readonly IMapper _mapper;
        readonly IGraphServiceProvider _graphServiceProvider;      

        public AzureUserService(IMapper mapper, IGraphServiceProvider graphServiceProvider)
        {
            _mapper = mapper;
            _graphServiceProvider = graphServiceProvider;
        }       

        public async Task<AzureUserDto> GetUserAsync(string id)
        {            
            var graphClient = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read" });

            var response = await graphClient.Users.Request().Filter($"Id eq '{id}'").GetAsync();

            var firstResponseItem = response.FirstOrDefault();

            return _mapper.Map<AzureUserDto>(firstResponseItem);
        }      
    }
}
