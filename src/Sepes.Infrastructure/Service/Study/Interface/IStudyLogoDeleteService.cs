using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyLogoDeleteService
    {
        Task DeleteAsync(Study study);          
    }
}