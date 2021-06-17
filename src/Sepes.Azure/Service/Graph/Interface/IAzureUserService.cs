using Sepes.Azure.Dto;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureUserService
    {
        Task<AzureUserDto> GetUserAsync(string id);
    }
}
