using System.Threading.Tasks;

namespace Sepes.RestApi.Services
{
    public interface IAuthService
    {
        Task<string> GenerateJSONWebToken();
    }
}
